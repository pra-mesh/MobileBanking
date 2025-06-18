using MobileBanking.Application.Contracts.Request.ISmart;
using MobileBanking.Application.Contracts.Response.ISmart;

namespace MobileBanking.Application.Services;
public interface IBalanceInquiry
{
    Task<BalanceInquiryResponse> GetBalance(BalanceInquiryRequest reqBalance);
}