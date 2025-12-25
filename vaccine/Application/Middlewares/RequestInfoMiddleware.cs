using vaccine.Application.Configurations;

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
        
        requestInfo.SetIP(ip);
        requestInfo.SetPort(port);
        requestInfo.SetCorrelationId(correlationId);
        await next(context);
    }
}