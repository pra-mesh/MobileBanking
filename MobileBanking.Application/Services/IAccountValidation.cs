
using MobileBanking.Application.Models;
using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Application.Services;

public interface IAccountValidation
{
    Task IsSingleAccount(string accountNo);
    Task<bool> HasSufficientBalance(string accountNO, bool isDebit, decimal transactionBalance);
    Task<string?> GetBranch(string accountNO);
    Task<AccountIdentifier> AccountStructure(string accountNO);
    Task<decimal> GeBalance(string accountNO);
    void InvalidAccount(string accountNo);
    void AccountCountValidation(List<AccountDetailDTO> accounts, string accountNo);
}
