using MobileBanking.Application.Mappings;
using MobileBanking.Application.Models;
using MobileBanking.Data.Repositories;

namespace MobileBanking.Application.Services;
public class MemberDetailService : IMemberDetailService
{
    private readonly IMemberDetailRepository _memberDetail;
    private readonly IBalanceInquiry _inquiry;

    public MemberDetailService(IMemberDetailRepository memberDetail, IBalanceInquiry inquiry)
    {
        _memberDetail = memberDetail;
        _inquiry = inquiry;
    }
    public async Task<List<ShareModel>> MemberShares(string Memberno)
    {
        var result = await _memberDetail.ShareList(Memberno);
        return [.. result.Select(DataToBusinessMapping.ToShareModel)];
    }
    public async Task<List<AccountDetailFullModel>> MembersAccount(string Memberno)
    {
        AccountQueryModel query = new AccountQueryModel { MemberNo = Memberno };
        return await _inquiry.GetAccountsDetailList(query);
    }
}
