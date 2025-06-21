using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Data.Repositories;
public interface IAccountRepository
{
    Task<int> AccountCount(string accountNo);
    Task<string> GetAccountBranch(string accountNO);
    Task<List<AccountDetailDTO>> GetAccountDetails(string accountno);
    Task<decimal> GetBalance(string accountNo);
    Task<decimal> GetDepBalance(string accountNo);
    Task<string> GetItemName(string fullAccountNo);
    Task<decimal> GetOpeningBalance(string accountNo, DateTime date);
}