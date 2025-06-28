using MobileBanking.Application.Mappings;
using MobileBanking.Application.Models;
using MobileBanking.Data.Repositories;

namespace MobileBanking.Application.Services;
public class LoanServices : ILoanServices
{
    private readonly ILoanRepository _loanRepository;

    public LoanServices(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }
    public async Task<LoanInfoModel> LoanDetail(AccountInquiryModel inquiry)
    {
        var result = await _loanRepository.LoanInfo(inquiry.accountNumber);

        if (result == null)
        {
            throw new AccountNotFoundException(inquiry.accountNumber);
        }
        return DataToBusinessMapping.ToLoanInfoModel(result);
    }
    public async Task<List<LoanStatementModel>> LoanStatement(FullStatmentInquiryModel statement)
    {
        var result = await _loanRepository.LoanStatements(statement.accountNumber, statement.fromDate, statement.toDate);
        return result.Select(DataToBusinessMapping.ToLoanStatement).ToList();
    }
    public async Task<List<LoanInfoModel>> AllLoanInfo(string Memberno)
    {
        var result = await _loanRepository.AllLoanInfo(Memberno);
        return [.. result.Select(DataToBusinessMapping.ToLoanInfoModel)];   //using collection expression
    }
}
