using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface IBalanceInquiry
{
    Task<List<AccountDetailFullModel>> GetAccountList(AllDetailsQueryModel req);
    Task<AccountDetailModel> GetBalance(BalanceInquiryModel reqBalance);
}