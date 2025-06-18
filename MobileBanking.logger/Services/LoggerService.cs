using Microsoft.Extensions.Logging;

namespace MobileBanking.Logger.Services;

public class LoggerService : ILoggerService
{
    private readonly ILogger<LoggerService> _logger;

    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger;
    }
    void ILoggerService.LogInformation(string message) => _logger.LogInformation(message);

    void ILoggerService.LogWarning(string message) => _logger.LogInformation(message);

    void ILoggerService.LogError(string message, Exception ex) => _logger.LogInformation(message);
}
