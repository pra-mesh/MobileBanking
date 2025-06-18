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

    public async Task<int> FundTransferAsync(FundTransferModel req)
    {

        await _accountValidation.HasSufficientBalance(req.srcAccount, true, req.amount);
        await _accountValidation.HasSufficientBalance(req.destAccount, false, req.amount);
        req.srcBranchId = await _accountValidation.GetBranch(req.srcAccount);
        req.destBranchId = await _accountValidation.GetBranch(req.destAccount);
        if (req.srcBranchId != req.destBranchId)
            return await InterbrachTransaction(req);
        else
            return await BranchlessTransaction(req);
    }

    private async Task<int> BranchlessTransaction(FundTransferModel req)
    {
        try
        {
            _unitOfWork.Begin();
            var mappedTransaction = BusinessToDataMapping.ToTransactionModel(req);
            await _transaction.GetTransNoAsync(mappedTransaction);
            int journalno = await _transaction.GenerateJournalNoAsync(BusinessToDataMapping.ToJournalNoDTO(req));
            if (journalno == 0)
                throw new Exception("System Error [Couldn't generate journalno]");

            _unitOfWork.Commit();
            return journalno;
        }
        catch (Exception e)
        {
            _unitOfWork.RollBack();
            throw new Exception($"System Error({e.Message})");
        }
    }

    private async Task<int> InterbrachTransaction(FundTransferModel req)
    {
        throw new NotImplementedException();
    }
}
