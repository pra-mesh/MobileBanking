using Dapper;
using Microsoft.Data.SqlClient;
using MobileBanking.Logger.Services;
using System.Data;

namespace MobileBanking.Data.Services.Connection;
public class SqlDataAccess : ISqlDataAccess
{
    private readonly IBaseSqlConnection _sqlConnection;
    private readonly ILoggerService _logger;
    private readonly string _connectionString;

    public SqlDataAccess(IBaseSqlConnection sqlConnection, ILoggerService logger)
    {
        _sqlConnection = sqlConnection;
        _logger = logger;
        _connectionString = _sqlConnection.GetConnectionString();
    }
    public async Task<List<T>> LoadData<T, U>(string storeProcedure, U parameters)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            //var start = DateTime.Now;
            //var stopwatch = Stopwatch.StartNew();
            //Console.WriteLine($"START at {DateTime.Now:HH:mm:ss.fff}");
            var rows = await connection.QueryAsync<T>(storeProcedure, parameters,
                 commandType: CommandType.StoredProcedure);
            List<T> data = rows.ToList();
            //Console.WriteLine($"END:  at {DateTime.Now:HH:mm:ss.fff}, took {stopwatch.ElapsedMilliseconds} ms");
            return data;
        }
    }
    public async Task SaveData<T>(string storeProcedure, T parameters)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            await connection.ExecuteAsync(storeProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }
    public async Task<List<T>> LoadDataQuery<T, U>(string commandText, U parameters)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            var rows = await connection.QueryAsync<T>(commandText, parameters,
                commandType: CommandType.Text);
            List<T> data = rows.ToList();
            return data;
        }
    }

    public async Task<T?> SingleDataQuery<T, U>(string commandText, U parameters)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            return await connection.QueryFirstOrDefaultAsync<T>(commandText, parameters, commandType: CommandType.Text);
        }
    }

    //Transactions
    private IDbConnection _dbConnection;
    private IDbTransaction _transaction;
    private bool isClosed = false;
    public void StartTransaction()
    {
        _dbConnection = new SqlConnection(_connectionString);
        _dbConnection.Open();
        _transaction = _dbConnection.BeginTransaction();
        isClosed = false;
    }
    public async Task<IEnumerable<T>> LoadDataTransactionQuery<T, U>(string commandText, U parameters)
    {
        return await _dbConnection.QueryAsync<T>(commandText, parameters, commandType: CommandType.Text, transaction: _transaction);
    }
    public async Task SaveDataTransactionQuery<T>(string commandText, T parameters)
    {
        await _dbConnection.ExecuteAsync(commandText, parameters, commandType: CommandType.Text, transaction: _transaction);
    }
    public async Task SaveDataTransactionProcedure<T>(string stroreProcedure, T parameters)
    {
        await _dbConnection.ExecuteAsync(stroreProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
    }
    public async Task<int> SaveDataScalarTransaction<T>(string commandText, T parameters)
    {
        var output = await _dbConnection.ExecuteScalarAsync(commandText, parameters, commandType: CommandType.Text, transaction: _transaction);
        return int.TryParse(output?.ToString(), out int id) ? id : 0;
    }

    public void CommitTransaction()
    {
        _transaction?.Commit();
        _dbConnection?.Close();
        isClosed = true;

    }
    public void RollBackTransaction()
    {
        _transaction?.Rollback();
        _dbConnection?.Close();
        isClosed = true;
    }
    public void Dispose()
    {
        if (isClosed == false)
        {
            try
            {
                RollBackTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        _dbConnection?.Close();

    }

}
