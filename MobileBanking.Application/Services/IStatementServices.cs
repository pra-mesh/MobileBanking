using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface IStatementServices
{
    Task<FullStatementModel> FullStatement(FullStatmentInquiryModel req);
    Task<MiniStatementModel> MiniStatement(MiniStatementInquiryModel req);
}