using Microsoft.Extensions.DependencyInjection;
using MobileBanking.Data.Repositories;
using MobileBanking.Data.Services;
using MobileBanking.Data.Services.Connection;

namespace MobileBanking.Data;
public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddScoped<IBaseSqlConnection, BaseSqlConnection>();
        services.AddScoped<ISqlDataAccess, SqlDataAccess>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IStatementRepository, StatementRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
