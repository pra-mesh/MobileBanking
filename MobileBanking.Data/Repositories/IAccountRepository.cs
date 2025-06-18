using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Data.Repositories;
public interface IAccountRepository
{
    Task<int> AccountCount(string accountNo);
    Task<List<AccountDetailDTO>> GetAccountDetails(string accountno);
    Task<decimal> GetBalance(string accountNo);
    Task<decimal> GetOpeningBalance(string accountNo, DateTime date);
}