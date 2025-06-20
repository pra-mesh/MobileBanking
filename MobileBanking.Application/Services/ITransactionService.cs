using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface ITransactionService
{
    Task<FundTransferedModel> FundTransferAsync(FundTransferModel req);
    Task<TransactionStatusModel> FundTransferStatus(TranactionStatusInquiryModel req);
    Task<ReversalStatusModel> TransactionReversal(ReversalRequestModel req);
}