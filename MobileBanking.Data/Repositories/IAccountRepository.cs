using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Models.RequestModels;

namespace MobileBanking.Data.Repositories;
public interface IAccountRepository
{
    Task<int> AccountCount(string accountNo);
    Task<List<AccountFullDetalDTO>> AllAccountFullDetails(AccountDetailPaged accountDetail);
    Task<string> GetAccountBranch(string accountNO);
    Task<int> GetAccountCount(AccountDetailPaged accountDetail);
    Task<List<AccountDetailDTO>> GetAccountDetails(string accountno);
    Task<decimal> GetBalance(string accountNo);
    Task<decimal> GetDepBalance(string accountNo);
    Task<string> GetItemName(string fullAccountNo);
    Task<decimal> GetOpeningBalance(string accountNo, DateTime date);
}