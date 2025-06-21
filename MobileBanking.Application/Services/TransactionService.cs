using MobileBanking.Application.Mappings;
using MobileBanking.Application.Models;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Repositories;
using MobileBanking.Shared.Utils;
//TODO: log on tblsilog
namespace MobileBanking.Application.Services;
public class TransactionService : ITransactionService
{

    private readonly ITransactionRepository _transaction;
    private readonly IAccountValidation _accountValidation;

    public TransactionService(ITransactionRepository transaction, IAccountValidation accountValidation)
    {
        _transaction = transaction;
        _accountValidation = accountValidation;
    }
    public async Task<TransactionStatusProcModel> FundTransferbyProc(FundTransferModel req)
    {
        TransactionProcDTO dto = BusinessToDataMapping.ToTransactionProcDTO(req);
        var result = await _transaction.TransactionByProc(dto);
        if (result.TransNoA == 0 || result.Journalno == 0)
        {
            string errorMessage = result.Message.ToLower();
            TransactionErrorMapping.GetError(errorMessage, req.destAccount, req.srcAccount);
        }
        return DataToBusinessMapping.ToTransactionStatusProcModel(result);

    }
    public async Task<FundTransferedModel> FundTransferbyProcWithBalance(FundTransferModel req)
    {
        var result = await FundTransferbyProc(req);
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
