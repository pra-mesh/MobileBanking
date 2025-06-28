using Microsoft.Extensions.Configuration;
using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Data.Services.Connection;
public interface IBaseSqlConnection
{
    IConfiguration Configuration { get; }
    string GetConnectionString();
    TenantModel GetTenantInfo();
}
