using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface IStatementServices
{
    Task<FullStatementModel> FullStatementBalance(FullStatmentInquiryModel req);
    Task<MiniStatementModel> MiniStatementBalance(MiniStatementInquiryModel req);
    Task<List<MiniStatement>> MiniStatement(MiniStatementInquiryModel req);
    Task<List<Statement>> FullStatements(FullStatmentInquiryModel req);
}