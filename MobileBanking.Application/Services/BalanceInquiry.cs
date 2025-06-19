using MobileBanking.Application.Mappings;
using MobileBanking.Application.Models;
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
    public async Task<AccountDetailModel> GetBalance(BalanceInquiryModel reqBalance)
    {
        await _valid.IsSingleAccount(reqBalance.accountNumber);
        var accounts = await _account.GetAccountDetails(reqBalance.accountNumber);

        var account = accounts.First();
        return DataToBusinessMapping.ToAccountDetailModel(account);



    }
}
