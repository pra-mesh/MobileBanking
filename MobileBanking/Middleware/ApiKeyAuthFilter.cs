using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MobileBanking.Application.AuthService;
using MobileBanking.Application.Constants;


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
            context.Result = new BadRequestResult();
            return;
        }
        if (!_apiKeyValidation.IsValidAPIKey(userApiKey))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
