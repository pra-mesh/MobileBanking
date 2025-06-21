using MobileBanking.Application.Mappings;
using MobileBanking.Application.Models;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Repositories;
using MobileBanking.Data.Services;
using MobileBanking.Shared.Utils;
using System.Diagnostics;
//TODO: log on tblsilog
namespace MobileBanking.Application.Services;
public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionRepository _transaction;
    private readonly IAccountValidation _accountValidation;

    public TransactionService(IUnitOfWork unitOfWork, ITransactionRepository transaction, IAccountValidation accountValidation)
    {
        _unitOfWork = unitOfWork;
        _transaction = transaction;
        _accountValidation = accountValidation;
    }

    public async Task<FundTransferedModel> FundTransferbyProc(FundTransferModel req)
    {
        TransactionProcDTO dto = BusinessToDataMapping.ToTransactionProcDTO(req);
        var result = await _transaction.TransactionByProc(dto);
        if (result.TransNoA == 0 || result.Journalno == 0)
        {
            string errorMessage = result.Message.ToLower();
            TransactionErrorMapping.GetError(errorMessage, req.destAccount, req.srcAccount);
        }
        decimal currentBalance = await _accountValidation.GeBalance(req.srcAccount);
        return new FundTransferedModel
        {
            Journalno = result.Journalno,
            BVRCNO = req.transCode ?? "",
            TransNoA = result.TransNoA,
            balance = currentBalance,
            transactionBalance = req.amount,
            transactionIdentifier = req.transCode ?? ""
        };

    }

    public async Task<FundTransferedModel> FundTransferAsync(FundTransferModel req)
    {
        //ISSUES: This task is taking too much time fix this
        await _accountValidation.HasSufficientBalance(req.srcAccount, true, req.amount);
        await _accountValidation.HasSufficientBalance(req.destAccount, false, req.amount);
        req.srcBranchId = await _accountValidation.GetBranch(req.srcAccount);
        req.destBranchId = await _accountValidation.GetBranch(req.destAccount);
        int journalno = 0;
        int transno = 0;
        if (req.srcBranchId != req.destBranchId)
            (journalno, transno) = await InterbrachTransaction(req);
        else
            (journalno, transno) = await BranchlessTransaction(req);
        decimal currentBalance = await _accountValidation.GeBalance(req.srcAccount);
        return new FundTransferedModel
        {
            Journalno = journalno,
            BVRCNO = req.transCode ?? "",
            TransNoA = transno,
            balance = currentBalance,
            transactionBalance = req.amount,
            transactionIdentifier = req.transCode ?? ""
        };
    }
    private async Task<(int, int)> BranchlessTransaction(FundTransferModel req)
    {
        try
        {
            _unitOfWork.Begin();
            var mappedTransaction = BusinessToDataMapping.ToTransactionModel(req);
            int transno = await _transaction.GetTransNoAsync(mappedTransaction);
            if (transno == 0) throw new Exception("Couldn't generate tranaction no.");
            int journalno = await _transaction.GenerateJournalNoAsync(BusinessToDataMapping.ToJournalNoDTO(req, false));
            if (journalno == 0) throw new Exception("Couldn't generate journalno.");
            await _transaction.InsertTransactionAsync(
                BusinessToDataMapping.ToTransactionDataDTO
                (req, await _accountValidation.AccountStructure(req.srcAccount), journalno, transno, true, req.srcBranchId));
            await _transaction.InsertTransactionAsync(
               BusinessToDataMapping.ToTransactionDataDTO
               (req, await _accountValidation.AccountStructure(req.destAccount), journalno, transno, false, req.srcBranchId));
            _unitOfWork.Commit();
            return (journalno, transno);
        }
        catch (Exception e)
        {
            _unitOfWork.RollBack();
            throw new Exception($"System Error [{e.Message}]");
        }
    }
    private async Task<(int, int)> InterbrachTransaction(FundTransferModel req)
    {
        try
        {
            //ISSUES: This task is taking too much time fix this
            var stopwatch = Stopwatch.StartNew();
            (AccountIdentifier headoffice,
                AccountIdentifier sourceBranch,
                AccountIdentifier destBranch,
                AccountIdentifier sourceAccount,
                AccountIdentifier destinationAccount) = await BranchAccountDetail(req);
            _unitOfWork.Begin();
            Console.WriteLine("Call SomeMethod {0} ms", stopwatch.ElapsedMilliseconds);
            var mappedTransaction = BusinessToDataMapping.ToTransactionModel(req);
            int transno = await _transaction.GetTransNoAsync(mappedTransaction);
            if (transno == 0) throw new Exception("Couldn't generate tranaction no.");
            //Source

            int journalno = await _transaction.GenerateJournalNoAsync(BusinessToDataMapping.ToJournalNoDTO(req, true));
            if (journalno == 0) throw new Exception("Couldn't generate journalno.");
            int defaultJournalno = journalno;
            await _transaction.InsertTransactionAsync(
                BusinessToDataMapping.ToTransactionDataDTO
                (req, sourceAccount, journalno, transno, true, req.srcBranchId));
            await _transaction.InsertTransactionAsync(
               BusinessToDataMapping.ToTransactionDataDTO
               (req, headoffice, journalno, transno, true, req.srcBranchId));
            //Destination
            journalno = await _transaction.GenerateJournalNoAsync(BusinessToDataMapping.ToJournalNoDTO(req, true));
            if (journalno == 0) throw new Exception("Couldn't generate journalno.");
            await _transaction.InsertTransactionAsync(
            BusinessToDataMapping.ToTransactionDataDTO
            (req, headoffice, journalno, transno, false, req.destBranchId));
            await _transaction.InsertTransactionAsync(
            BusinessToDataMapping.ToTransactionDataDTO
            (req, destinationAccount, journalno, transno, false, req.destBranchId));

            //headoffice
            journalno = await _transaction.GenerateJournalNoAsync(BusinessToDataMapping.ToJournalNoDTO(req, true));
            if (journalno == 0) throw new Exception("Couldn't generate journalno.");
            await _transaction.InsertTransactionAsync(
            BusinessToDataMapping.ToTransactionDataDTO
            (req, sourceBranch, journalno, transno, true, "00"));
            await _transaction.InsertTransactionAsync(
            BusinessToDataMapping.ToTransactionDataDTO
            (req, destBranch, journalno, transno, false, "00"));


            _unitOfWork.Commit();
            return (defaultJournalno, transno);
        }
        catch (Exception e)
        {
            _unitOfWork.RollBack();
            throw new Exception($"System Error [{e.Message}]");
        }
    }
    private async Task<(AccountIdentifier headoffice, AccountIdentifier sourceBranch, AccountIdentifier destBranch,
        AccountIdentifier sourceAccount, AccountIdentifier destinationAccount)>
        BranchAccountDetail(FundTransferModel req)
    {
        var headofficeTask = _accountValidation.AccountStructure("1202000");
        var sourceBranchTask = _accountValidation.AccountStructure($"12020{req.srcBranchId}");
        var destBranchTask = _accountValidation.AccountStructure($"12020{req.destBranchId}");
        var sourceAccountTask = _accountValidation.AccountStructure(req.srcAccount);
        var destinationAccountTask = _accountValidation.AccountStructure(req.destAccount);
        await Task.WhenAll(headofficeTask, sourceAccountTask, sourceBranchTask, destBranchTask, destinationAccountTask);

        AccountIdentifier headoffice = await headofficeTask;
        AccountIdentifier sourceBranch = await sourceBranchTask;
        AccountIdentifier destBranch = await destBranchTask;
        AccountIdentifier sourceAccount = await sourceAccountTask;
        AccountIdentifier destinationAccount = await destinationAccountTask;
        return (headoffice, sourceBranch, destBranch, sourceAccount, destinationAccount);
    }

    public async Task<TransactionStatusModel> FundTransferStatus(TranactionStatusInquiryModel req)
    {
        if (req.JournalNo != 0)
            return DataToBusinessMapping.ToFundTransferedModel
                (await _transaction.SearchTransactionByJournalNo(req.JournalNo));
        if (!string.IsNullOrEmpty(req.BVRCNO))
            return DataToBusinessMapping.ToFundTransferedModel
                (await _transaction.SearchTransactionByBVRCNO(req.BVRCNO));
        throw new Exception("System Error [Transaction not found]");
    }
    public async Task<ReversalStatusModel> TransactionReversal(ReversalRequestModel req)
    {
        List<string>? journalnos = null;
        if (req.JournalNo != 0)
            journalnos = await _transaction.JournalnosBYJournalno(req.JournalNo, req.enteredBY);
        if (!string.IsNullOrWhiteSpace(req.BVRCNO))
            journalnos = await _transaction.JournalnosByBVRCNO(req.BVRCNO, req.enteredBY);
        if (journalnos is null || journalnos.Count < 1)
            throw new Exception("Unable To Process [Transaction not found]");
        var result = await _transaction.ReverseTransaction(BusinessToDataMapping.ToReverseTansactionDTO(req));
        if (result.TransNoA == 0 || result.Journalno == 0)
            throw new Exception($"Unable To Process [{result.Message}]");
        return DataToBusinessMapping.ToReversalStatusModel(result);

    }
}
