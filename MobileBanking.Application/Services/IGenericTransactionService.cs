using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface IGenericTransactionService
{
    Task<FundTransferedModel> FundTransferAsync(FundTransferModel req);
}