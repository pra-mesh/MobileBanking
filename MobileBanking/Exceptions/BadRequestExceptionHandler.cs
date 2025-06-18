using Microsoft.AspNetCore.Diagnostics;
using MobileBanking.Application.Contracts.Response;
using MobileBanking.Logger.Services;
using MobileBanking.Shared.Utils;

namespace MobileBanking.Exceptions;

public class BadRequestExceptionHandler : IExceptionHandler
{
    private readonly ILoggerService _logger;

    public BadRequestExceptionHandler(ILoggerService logger)
    {
        _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badRequest)
        {
            return false;
        }
        _logger.LogError(badRequest.Message, badRequest);
        var problemDetails = new BaseResponse<string>
        {
            statusCode = ResponseCode.GetResponseCode(badRequest),
            data = badRequest.Message.Replace("_", " ")

        };

        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        await httpContext.Response
           .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;

    }
}
