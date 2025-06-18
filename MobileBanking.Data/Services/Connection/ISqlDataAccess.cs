namespace MobileBanking.Data.Services.Connection;
public interface ISqlDataAccess
{
    Task<List<T>> LoadData<T, U>(string storeProcedure, U parameters);
    Task<List<T>> LoadDataQuery<T, U>(string commandText, U parameters);
    Task<T> SingleDataQuery<T, U>(string commandText, U parameters);
}
