using Microsoft.AspNetCore.Mvc;
using vaccine.Application.Configurations;
using vaccine.Application.Constants;

namespace vaccine.Application.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IRequestInfo _requestInfo;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IRequestInfo requestInfo)
    {
        _logger = logger;
        _requestInfo = requestInfo;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception. Path: {Path} | CorrelationId: {CorrelationId}",
                context.Request.Path,
                _requestInfo.CorrelationId
            );

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response =  new ProblemDetails
        {
            Type = ProblemDetailTypes.InternalError,
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "An unexpected error occurred."
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}
