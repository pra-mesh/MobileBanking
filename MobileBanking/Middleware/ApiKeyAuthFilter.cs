using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MobileBanking.Application.AuthService;
using MobileBanking.Application.Constants;
using MobileBanking.Models.Response;


namespace MobileBanking.Middleware;

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private readonly IApiKeyValidation _apiKeyValidation;

    public ApiKeyAuthFilter(IApiKeyValidation apiKeyValidation)
    {
        _apiKeyValidation = apiKeyValidation;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userApiKey = context.HttpContext.Request.Headers[ApiKeyConstants.ApiKeyHeaderName];
        if (string.IsNullOrEmpty(userApiKey))
        {
            var response = new BaseResponse<string>
            {
                data = "API key is required",
                statusCode = "05"
            };
            context.Result = new BadRequestObjectResult(response);
            return;
        }
        if (!_apiKeyValidation.IsValidAPIKey(userApiKey))
        {
            var response = new BaseResponse<string>
            {
                data = "The provided API key is not valid.",
                statusCode = "05"
            };
            context.Result = new UnauthorizedObjectResult(response);
            return;
        }
    }
}
