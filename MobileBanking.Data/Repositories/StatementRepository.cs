using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Services.Connection;

namespace MobileBanking.Data.Repositories;
public class StatementRepository : IStatementRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public StatementRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<FullStatementDTO>> FullStatement(string accountNo, DateTime fromDate, DateTime toDate) =>
       await _sqlDataAccess.LoadData<FullStatementDTO, dynamic>("SP_MBFullStatment", new { accountNo, fromDate, toDate });

    public async Task<List<MiniStatementDTO>> MiniStatement(string accountNo, int noOfTransaction) =>
       await _sqlDataAccess.LoadData<MiniStatementDTO, dynamic>("SP_MBMiniStatment", new { accountNo, noOfTransaction });
}
