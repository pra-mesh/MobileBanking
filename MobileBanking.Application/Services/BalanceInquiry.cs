using MobileBanking.Application.Mappings;
using MobileBanking.Application.Models;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Models.RequestModels;
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
    public async Task<AccountDetailModel> GetBalance(AccountInquiryModel reqBalance)
    {
        _valid.InvalidAccount(reqBalance.accountNumber);
        var accounts = await _account.GetAccountDetails(reqBalance.accountNumber);
        _valid.AccountCountValidation(accounts, reqBalance.accountNumber);
        var account = accounts.First();
        return DataToBusinessMapping.ToAccountDetailModel(account);

    }

    public async Task<List<AccountDetailFullModel>> GetAccountsDetailList(AccountQueryModel req)
    {
        var query = BusinessToDataMapping.ToAccountQueryDTO(req);
        int count = await _account.GetAccountCount(query);
        var tasks = new List<Task<List<AccountFullDetalDTO>>>();
        for (int i = 0; i < Math.Ceiling((double)count / 2); i++)
        {
            var pagedQuery = new AccountPagedQueryDTO
            {
                Offset = 2 * i,
                Limit = 2,
                AccountNumber = query.AccountNumber,
                MobileNumber = query.MobileNumber,
                MemberNo = query.MemberNo,
            };
            tasks.Add(_account.AllAccountFullDetails(pagedQuery));
        }
        var results = await Task.WhenAll(tasks);
        var allAccounts = results.SelectMany(r => r).ToList().Distinct();
        return allAccounts.Select(DataToBusinessMapping.ToAccountDetailFullModel).ToList();
    }

    public async Task<List<AccountModel>> GetAccounts(AccountQueryModel req)
    {
        var query = BusinessToDataMapping.ToAccountQueryDTO(req);
        var result = await _account.AccountsIDList(query);
        return result.Select(DataToBusinessMapping.ToAccountModel).ToList();
    }
}
