using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface ILoanServices
{
    Task<LoanInfoModel> LoanDetail(AccountInquiryModel inquiry);
    Task<List<LoanStatementModel>> LoanStatement(FullStatmentInquiryModel statement);
}