using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Shared.Interface;
using System.Data;

namespace MobileBanking.Data.Services.Connection;
public class BaseSqlConnection : IBaseSqlConnection
{
    private readonly ITenantProvider _tenant;

    public BaseSqlConnection(IConfiguration configuration, ITenantProvider tenant)
    {
        Configuration = configuration;
        _tenant = tenant;
    }
    public IConfiguration Configuration { get; }

    public string GetConnectionString()
    {
        return BuildConnectionString(BaseConnectionString(), GetTenantInfo()).ToString();
    }

    private string BaseConnectionString()
    {
        var connectionString = new SqlConnectionStringBuilder(Configuration.GetConnectionString("DbConnection"));
        connectionString.Password = Configuration.GetValue<string>("DbCredentials:Password");
        connectionString.UserID = Configuration.GetValue<string>("DbCredentials:userName");
        return connectionString.ToString();
    }

    public TenantModel GetTenantInfo()
    {
        string connectionString = BaseConnectionString();
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            var tenant = connection.QueryFirstOrDefault<TenantModel>(
                "select * from Mbank where ClientId = @ClientId",
                new { ClientId = _tenant.GetTenantName() },
                commandType: CommandType.Text);

            if (tenant is null || tenant.CurrentDB is null || tenant.ServerIP is null)
            {
                throw new Exception("Unable_To_Process(Invalid Tenant)");
            }

            return tenant;
        }
    }

    private SqlConnectionStringBuilder BuildConnectionString(string connectionString, TenantModel tenant)
    {
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            DataSource = tenant.ServerIP,
            InitialCatalog = tenant.CurrentDB
        };
        return builder;
    }

}
