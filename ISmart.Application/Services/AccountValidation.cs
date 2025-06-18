using MobileBanking.Data.Repositories;

namespace MobileBanking.Application.Services;
public class AccountValidation : IAccountValidation
{
    private readonly IAccountRepository _account;
    string[] credit = { "010", "020", "030", "040", "050", "060", "070", "160", "180", "210", "230", "250" };
    public AccountValidation(IAccountRepository account)
    {
        _account = account;
    }
    public async Task IsSingleAccount(string accountNo)
    {
        int accountCount = await _account.AccountCount(accountNo);
        if (accountCount == 0 || accountCount < 1)
            throw new AccountNotFoundException(accountNo);
        if (accountCount > 1)
            throw new MultipleAccountsFoundException(accountNo);

    }

    public async Task<bool> HasSufficientBalance(string accountNO, bool isDebit, decimal transactionBalance)
    {
        await IsSingleAccount(accountNO);
        int balanceSide = 1;
        string mainAccountNO = accountNO.Substring(0, 3);
        if (credit.Contains(mainAccountNO))
            balanceSide = -1;

        decimal balance = 0;
        if (mainAccountNO != "030")
            balance = decimal.Multiply(balanceSide, await _account.GetBalance(accountNO));
        else
        {
            var accountsDetail = await _account.GetAccountDetails(accountNO);
            var accountDetail = accountsDetail.FirstOrDefault();
            if (accountDetail is null) throw new AccountNotFoundException(accountNO);
            balance = accountDetail.Balance - accountDetail.Gamt - accountDetail.Lamt - accountDetail.MinBal;
        }
        if (isDebit && balanceSide == -1 && transactionBalance > balance)
            throw new InsufficientBalanceException(accountNO);
        if (!isDebit && balance == 1 && transactionBalance > balance)
            throw new InsufficientBalanceException(accountNO);
        return true;
    }

    public async Task<string> GetBranch(string accountNO) => await _account.GetAccountBranch(accountNO);
}
