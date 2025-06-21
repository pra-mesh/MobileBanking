using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface ITransactionService
{
    Task<TransactionStatusProcModel> FundTransferbyProc(FundTransferModel req);
    Task<FundTransferedModel> FundTransferbyProcWithBalance(FundTransferModel req);
    Task<TransactionStatusModel> FundTransferStatus(TranactionStatusInquiryModel req);
    Task<ReversalStatusModel> TransactionReversal(ReversalRequestModel req);
}