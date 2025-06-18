
namespace MobileBanking.Application.Services;

public interface IAccountValidation
{
    Task IsSingleAccount(string accountNo);
    Task<bool> HasSufficientBalance(string accountNO, bool isDebit, decimal transactionBalance);
    Task<string> GetBranch(string accountNO);
}
