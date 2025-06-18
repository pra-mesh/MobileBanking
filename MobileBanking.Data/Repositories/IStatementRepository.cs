using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Data.Repositories;
public interface IStatementRepository
{
    Task<List<FullStatementDTO>> FullStatement(string accountNo, DateTime fromDate, DateTime toDate);
    Task<List<MiniStatementDTO>> MiniStatement(string accountNo, int noOfTransaction);
}