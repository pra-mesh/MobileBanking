using Microsoft.AspNetCore.Mvc;
using MobileBanking.Middleware;

namespace MobileBanking.Attributes;

public class ApiKeyAttribute : ServiceFilterAttribute
{
    public ApiKeyAttribute() : base(typeof(ApiKeyAuthFilter))
    {

    }
}
