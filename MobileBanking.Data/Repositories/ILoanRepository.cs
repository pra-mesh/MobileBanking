using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Data.Repositories;
public interface ILoanRepository
{
    Task<List<LoanInfoDTO>> AllLoanInfo(string memberno);
    Task<LoanInfoDTO> LoanInfo(string accountNumber);
    Task<List<LoanStatementDTO>> LoanStatements(string accountNumber, DateTime fromDate, DateTime toDate);
}