using MobileBanking.Application.Mappings;
using MobileBanking.Application.Models;
using MobileBanking.Data.Repositories;
using MobileBanking.Data.Services;

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

    public async Task<FundTransferedModel> FundTransferAsync(FundTransferModel req)
    {

        await _accountValidation.HasSufficientBalance(req.srcAccount, true, req.amount);
        await _accountValidation.HasSufficientBalance(req.destAccount, false, req.amount);
        req.srcBranchId = await _accountValidation.GetBranch(req.srcAccount);
        req.destBranchId = await _accountValidation.GetBranch(req.destAccount);
        int journalno = 0;
        if (req.srcBranchId != req.destBranchId)
            journalno = await InterbrachTransaction(req);
        else
            journalno = await BranchlessTransaction(req);
        decimal currentBalance = await _accountValidation.GeBalance(req.srcAccount);
        return new FundTransferedModel
        {
            journalno = journalno,
            balance = currentBalance,
            transactionBalance = req.amount,
            transactionIdentifier = req.transCode ?? ""
        };
    }

    private async Task<int> BranchlessTransaction(FundTransferModel req)
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
                (req, await _accountValidation.AccountStructure(req.srcAccount), journalno, transno, true));
            await _transaction.InsertTransactionAsync(
               BusinessToDataMapping.ToTransactionDataDTO
               (req, await _accountValidation.AccountStructure(req.destAccount), journalno, transno, false));
            _unitOfWork.Commit();
            return journalno;
        }
        catch (Exception e)
        {
            _unitOfWork.RollBack();
            throw new Exception($"System Error [{e.Message}]");
        }
    }

    private async Task<int> InterbrachTransaction(FundTransferModel req)
    {
        try
        {
            AccountIdentifier headoffice = await _accountValidation.AccountStructure("1202000");
            AccountIdentifier sourceBranch = await _accountValidation.AccountStructure($"12020{req.srcBranchId}");
            AccountIdentifier destBranch = await _accountValidation.AccountStructure($"12020{req.destBranchId}");
            AccountIdentifier sourceAccount = await _accountValidation.AccountStructure(req.srcAccount);
            AccountIdentifier destinationAccount = await _accountValidation.AccountStructure(req.destAccount);

            _unitOfWork.Begin();
            var mappedTransaction = BusinessToDataMapping.ToTransactionModel(req);
            int transno = await _transaction.GetTransNoAsync(mappedTransaction);
            if (transno == 0) throw new Exception("Couldn't generate tranaction no.");
            //Source
            int journalno = await _transaction.GenerateJournalNoAsync(BusinessToDataMapping.ToJournalNoDTO(req, true));
            if (journalno == 0) throw new Exception("Couldn't generate journalno.");
            int defaultJournalno = journalno;
            await _transaction.InsertTransactionAsync(
                BusinessToDataMapping.ToTransactionDataDTO
                (req, sourceAccount, journalno, transno, true));
            await _transaction.InsertTransactionAsync(
               BusinessToDataMapping.ToTransactionDataDTO
               (req, headoffice, journalno, transno, false));
            //Destination
            journalno = await _transaction.GenerateJournalNoAsync(BusinessToDataMapping.ToJournalNoDTO(req, true));
            if (journalno == 0) throw new Exception("Couldn't generate journalno.");
            await _transaction.InsertTransactionAsync(
            BusinessToDataMapping.ToTransactionDataDTO
            (req, headoffice, journalno, transno, true));
            await _transaction.InsertTransactionAsync(
            BusinessToDataMapping.ToTransactionDataDTO
            (req, destinationAccount, journalno, transno, false));

            //headoffice
            await _transaction.InsertTransactionAsync(
            BusinessToDataMapping.ToTransactionDataDTO
            (req, sourceBranch, journalno, transno, true));
            await _transaction.InsertTransactionAsync(
            BusinessToDataMapping.ToTransactionDataDTO
            (req, destBranch, journalno, transno, false));


            _unitOfWork.Commit();
            return defaultJournalno;
        }
        catch (Exception e)
        {
            _unitOfWork.RollBack();
            throw new Exception($"System Error [{e.Message}]");
        }
    }
}
