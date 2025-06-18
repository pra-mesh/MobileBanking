using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface ITransactionService
{
    Task<int> FundTransferAsync(FundTransferModel req);
}