using Microsoft.Extensions.Configuration;
using MobileBanking.Data.Services.Connection;
using MobileBanking.Logger.Services;

namespace MobileBanking.Application.AuthService;
public class ApiKeyValidation : IApiKeyValidation
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _logger;
    private readonly IBaseSqlConnection _baseSql;

    public ApiKeyValidation(IConfiguration configuration, ILoggerService logger, IBaseSqlConnection baseSql)
    {
        _configuration = configuration;
        _logger = logger;
        _baseSql = baseSql;
    }
    public bool IsValidAPIKey(string userApiKey)
    {
        var v = _baseSql.GetTenantInfo();
        var apiKey = v.ApiKey;
        bool isValid = !string.IsNullOrWhiteSpace(userApiKey) && !string.IsNullOrWhiteSpace(apiKey)
            && apiKey == userApiKey;
        if (!isValid)
        {
            _logger.LogWarning("API key validation failed: Invalid Key");
            return false;
        }
        _logger.LogInformation("API key validated successfully.");
        return true;

    }
}
