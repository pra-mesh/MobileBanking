using Microsoft.AspNetCore.Http;
using MobileBanking.Logger.Services;
using MobileBanking.Shared.Interface;

namespace MobileBanking.Application.MultiTenancy;
public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _context;
    private readonly ILoggerService _logger;

    public TenantProvider(IHttpContextAccessor context, ILoggerService logger)
    {
        _context = context;
        _logger = logger;
    }
    public string GetTenantName()
    {
        string tenant = _context.HttpContext?.Items["TenantName"]?.ToString() ?? "";
        if (string.IsNullOrWhiteSpace(tenant))
            _logger.LogError("Tenant not found", new Exception("Invalid tenant"));
        return tenant;
    }

}
