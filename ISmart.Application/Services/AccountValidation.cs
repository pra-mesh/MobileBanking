using MobileBanking.Data.Repositories;

namespace MobileBanking.Application.Services;
public class AccountValidation : IAccountValidation
{
    private readonly IAccountRepository _account;

    public AccountValidation(IAccountRepository account)
    {
        _account = account;
    }
    public async Task IsSingleAccount(string accountNo)
    {
        int accountCount = await _account.AccountCount(accountNo);
        if (accountCount == 0 || accountCount < 1)
            throw new AccountNotFoundException();
        if (accountCount > 1)
            throw new MultipleAccountsFoundException();

    }
}
