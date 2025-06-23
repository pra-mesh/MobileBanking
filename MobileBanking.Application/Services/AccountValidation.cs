using MobileBanking.Application.Models;
using MobileBanking.Data.Models.DTOs;
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
        if (accountNo.Length < 6)
            throw new AccountNotFoundException(accountNo);
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
        decimal balance = await GeBalance(accountNO);
        if (mainAccountNO != "030")
            balance = Decimal.Multiply(balanceSide, balance);
        if (isDebit && balanceSide == -1 && transactionBalance > balance)
            throw new InsufficientBalanceException(accountNO);
        if (!isDebit && balance == 1 && transactionBalance > balance)
            throw new InsufficientBalanceException(accountNO);
        return true;
    }
    public async Task<decimal> GeBalance(string accountNO)
    {
        string mainAccountNO = accountNO.Substring(0, 3);
        if (mainAccountNO != "030")
            return await _account.GetBalance(accountNO);
        else
        {
            return await _account.GetDepBalance(accountNO);
        }


    }
    public async Task<string> GetBranch(string accountNO) => await _account.GetAccountBranch(accountNO);
    public async Task<AccountIdentifier> AccountStructure(string accountNO) =>
        new AccountIdentifier
        {
            Mano = accountNO.Substring(0, 3),
            Acno = $"{accountNO.Substring(0, 3)}.{accountNO.Substring(3, 2)}",
            ItemCode = accountNO.Substring(5),
            ItemName = await _account.GetItemName(accountNO)
        };
    public void AccountCountValidation(List<AccountDetailDTO> accounts, string accountNo)
    {
        if (accounts.Count == 0 || accounts.Count < 1)
            throw new AccountNotFoundException(accountNo);
        if (accounts.Count > 1)
            throw new MultipleAccountsFoundException(accountNo);
    }
    public void InvalidAccount(string accountNo)
    {
        if (accountNo.Length < 6)
            throw new InvalidAccountException(accountNo);
    }
}
