using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface IBalanceInquiry
{
    Task<AccountDetailModel> GetBalance(BalanceInquiryModel reqBalance);
}