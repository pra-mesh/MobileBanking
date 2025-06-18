using Microsoft.AspNetCore.Diagnostics;
using MobileBanking.Application.Contracts.Response;
using MobileBanking.Logger.Services;
using MobileBanking.Shared.Utils;

namespace MobileBanking.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILoggerService _logger;

    public GlobalExceptionHandler(ILoggerService logger)
    {
        _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError($"Exception occurred: {exception.Message}", exception);
        var problemDetails = new BaseResponse<string>
        {
            statusCode = ResponseCode.GetResponseCode(exception),
            data = "System Error " + exception.Message
        };
        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;

    }

}
