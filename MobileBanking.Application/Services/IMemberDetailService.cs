using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface IMemberDetailService
{
    Task<List<AccountDetailFullModel>> MembersAccount(string Memberno);
    Task<List<ShareModel>> MemberShares(string Memberno);
}