using Microsoft.Extensions.DependencyInjection;
using MobileBanking.Application.AuthService;
using MobileBanking.Application.MultiTenancy;
using MobileBanking.Application.Services;
using MobileBanking.Data;
using MobileBanking.Shared.Interface;

namespace MobileBanking.Application;
public static class ApplicationServiceCollection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IApiKeyValidation, ApiKeyValidation>();
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<IBalanceInquiry, BalanceInquiry>();
        services.AddScoped<IAccountValidation, AccountValidation>();
        services.AddScoped<IStatementServices, StatementServices>();
        services.AddDataServices();
        return services;
    }
}
