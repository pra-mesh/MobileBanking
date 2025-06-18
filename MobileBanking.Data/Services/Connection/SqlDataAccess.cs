using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MobileBanking.Data.Services.Connection;
public class SqlDataAccess : ISqlDataAccess
{
    private readonly IBaseSqlConnection _sqlConnection;

    public SqlDataAccess(IBaseSqlConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }
    public async Task<List<T>> LoadData<T, U>(string storeProcedure, U parameters)
    {
        string connectionString = _sqlConnection.GetConnectionString();
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            var rows = await connection.QueryAsync<T>(storeProcedure, parameters,
                 commandType: CommandType.StoredProcedure);
            List<T> data = rows.ToList();
            return data;
        }
    }

    public async Task<List<T>> LoadDataQuery<T, U>(string commandText, U parameters)
    {
        string connectionString = _sqlConnection.GetConnectionString();
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            var rows = await connection.QueryAsync<T>(commandText, parameters,
                commandType: CommandType.Text);
            List<T> data = rows.ToList();
            return data;
        }
    }

    public async Task<T> SingleDataQuery<T, U>(string commandText, U parameters)
    {
        string connectionString = _sqlConnection.GetConnectionString();
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            return await connection.QueryFirstAsync<T>(commandText, parameters, commandType: CommandType.Text);
        }
    }
}
