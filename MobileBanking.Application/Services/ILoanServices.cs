using MobileBanking.Application.Models;

namespace MobileBanking.Application.Services;
public interface ILoanServices
{
    Task<List<LoanInfoModel>> AllLoanInfo(string Memberno);
    Task<LoanInfoModel> LoanDetail(AccountInquiryModel inquiry);
    Task<List<LoanStatementModel>> LoanStatement(FullStatmentInquiryModel statement);
}