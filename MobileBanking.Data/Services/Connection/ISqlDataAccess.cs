
namespace MobileBanking.Data.Services.Connection;
public interface ISqlDataAccess : IDisposable
{
    void CommitTransaction();
    Task<List<T>> LoadData<T, U>(string storeProcedure, U parameters);
    Task<List<T>> LoadDataQuery<T, U>(string commandText, U parameters);
    Task<IEnumerable<T>> LoadDataTransactionQuery<T, U>(string commandText, U parameters);
    void RollBackTransaction();
    Task<int> SaveDataScalarTransaction<T>(string commantText, T parameters);
    Task SaveDataTransactionProcedure<T>(string stroreProceddure, T parameters);
    Task SaveDataTransactionQuery<T>(string commandText, T parameters);
    Task<T> SingleDataQuery<T, U>(string commandText, U parameters);
    void StartTransaction();
}
