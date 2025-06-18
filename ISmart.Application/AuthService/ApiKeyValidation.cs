using Microsoft.Extensions.Configuration;
using MobileBanking.Application.Constants;
using MobileBanking.Logger.Services;

namespace MobileBanking.Application.AuthService;
public class ApiKeyValidation : IApiKeyValidation
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _logger;

    public ApiKeyValidation(IConfiguration configuration, ILoggerService logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    public bool IsValidAPIKey(string userApiKey)
    {

        var apiKey = _configuration.GetValue<string>(ApiKeyConstants.ApiKeyName);
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
