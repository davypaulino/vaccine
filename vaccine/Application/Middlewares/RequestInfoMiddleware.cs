using System.Security.Claims;
using vaccine.Application.Configurations;
using vaccine.Application.Constants;

namespace vaccine.Application.Middlewares;

public class RequestInfoMiddleware : IMiddleware
{
    private readonly ILogger<RequestInfoMiddleware> _logger;

    public RequestInfoMiddleware(ILogger<RequestInfoMiddleware> logger)
    {
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var requestInfo = context.RequestServices.GetRequiredService<IRequestInfo>();
        
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                 ?? context.Connection.RemoteIpAddress?.ToString();

        var port = context.Request.Host.Port?.ToString();

        Guid.TryParse(context.Request.Headers["X-Correlation-Id"].FirstOrDefault(), out var correlationId);

        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var user = context.User;

            string? userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? userName = user.FindFirst(ClaimTypes.Name)?.Value;
            string? email = user.FindFirst(ClaimTypes.Email)?.Value;
            string? role = user.FindFirst(ClaimTypes.Role)?.Value;
            string? personId = user.FindFirst(VaccineClaimTypes.PersonId)?.Value;

            requestInfo.SetUserInfo(userId, userName, email, role, personId);
        }

        requestInfo.SetIP(ip);
        requestInfo.SetPort(port);
        requestInfo.SetCorrelationId(correlationId);
        await next(context);
    }
}