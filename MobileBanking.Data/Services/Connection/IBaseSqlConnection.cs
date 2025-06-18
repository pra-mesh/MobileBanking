using Microsoft.Extensions.Configuration;

namespace MobileBanking.Data.Services.Connection;
public interface IBaseSqlConnection
{
    IConfiguration Configuration { get; }
    string GetConnectionString();

}
