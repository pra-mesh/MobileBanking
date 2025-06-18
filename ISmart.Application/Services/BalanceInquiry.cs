using MobileBanking.Application.Contracts.Request.ISmart;
using MobileBanking.Application.Contracts.Response.ISmart;
using MobileBanking.Data.Repositories;

namespace MobileBanking.Application.Services;
public class BalanceInquiry : IBalanceInquiry
{
    private readonly IAccountRepository _account;
    private readonly IAccountValidation _valid;

    public BalanceInquiry(IAccountRepository account, IAccountValidation valid)
    {
        _account = account;
        _valid = valid;
    }
    public async Task<BalanceInquiryResponse> GetBalance(BalanceInquiryRequest reqBalance)
    {
        await _valid.IsSingleAccount(reqBalance.accountNumber);
        var accounts = await _account.GetAccountDetails(reqBalance.accountNumber);

        var account = accounts.First();
        return new BalanceInquiryResponse
        {
            isoResponseCode = "00",
            availableBalance = account.Balance,
            minimumBalance = account.MinBal
        };

    }
}
