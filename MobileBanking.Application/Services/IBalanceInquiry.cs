using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface IBalanceInquiry
{
    Task<List<AccountModel>> GetAccounts(AccountQueryModel req);
    Task<List<AccountDetailFullModel>> GetAccountsDetailList(AccountQueryModel req);
    Task<AccountDetailModel> GetBalance(AccountInquiryModel reqBalance);
}