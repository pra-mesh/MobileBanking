

using MobileBanking.Models.Response;

namespace MobileBanking.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _requestDelegate;

    public TenantMiddleware(RequestDelegate requestDelegate)
    {
        _requestDelegate = requestDelegate;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("tenant", out var extractedTenant))
        {
            context.Response.StatusCode = 403;

            await context.Response.WriteAsJsonAsync(new BaseResponse<string>
            {
                statusCode = "96",
                data = "Missing Tenant"
            }
            );
            return;
        }
        context.Items["TenantName"] = extractedTenant.ToString();

        await _requestDelegate(context);
    }
}
