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
    public async Task<AccountDetailModel> GetBalance(BalanceInquiryModel reqBalance)
    {
        _valid.InvalidAccount(reqBalance.accountNumber);
        var accounts = await _account.GetAccountDetails(reqBalance.accountNumber);
        _valid.AccountCountValidation(accounts, reqBalance.accountNumber);
        var account = accounts.First();
        return DataToBusinessMapping.ToAccountDetailModel(account);

    }

    public async Task<List<AccountDetailFullModel>> GetAccountList(AllDetailsQueryModel req)
    {
        var query = BusinessToDataMapping.ToAccountDetailPagedDTO(req);
        int count = await _account.GetAccountCount(query);
        var tasks = new List<Task<List<AccountFullDetalDTO>>>();
        for (int i = 0; i < count / 2; i++)
        {
            var pagedQuery = new AccountDetailPaged
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
}
