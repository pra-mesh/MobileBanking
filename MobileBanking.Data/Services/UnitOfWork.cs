using MobileBanking.Data.Services.Connection;

namespace MobileBanking.Data.Services;
public class UnitOfWork : IUnitOfWork
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public UnitOfWork(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }
    public void Begin()
    {
        _sqlDataAccess.StartTransaction();
    }

    public void Commit()
    {
        _sqlDataAccess.CommitTransaction();
    }

    public void RollBack()
    {
        _sqlDataAccess.RollBackTransaction();
    }

    public void Dispose()
    {
        _sqlDataAccess.Dispose();
    }
}
