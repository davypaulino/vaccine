using System.Text.Json;
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
            if (context.Response.HasStarted)
                return;

            switch (ex)
            {
                case BadHttpRequestException badRequest:
                    _logger.LogWarning(badRequest,
                        "Bad request. Path: {Path} | CorrelationId: {CorrelationId}",
                        context.Request.Path,
                        _requestInfo.CorrelationId
                    );

                    await WriteBadRequest(context, badRequest);
                    break;

                default:
                    _logger.LogError(ex,
                        "Unhandled exception. Path: {Path} | CorrelationId: {CorrelationId}",
                        context.Request.Path,
                        _requestInfo.CorrelationId
                    );

                    await WriteInternalServerError(context);
                    break;
            }
        }
    }
    
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        return exception switch
        {
            BadHttpRequestException badRequest =>
                WriteBadRequest(context, badRequest),

            _ =>
                WriteInternalServerError(context)
        };
    }


    private Task WriteInternalServerError(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new ProblemDetails
        {
            Type = ProblemDetailTypes.InternalError,
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "An unexpected error occurred."
        };

        return context.Response.WriteAsJsonAsync(response);
    }
    
    private Task WriteBadRequest(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var problem = new ProblemDetails
        {
            Type = ProblemDetailTypes.BadRequest,
            Title = "Invalid request",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"""
                      The request body is invalid or malformed. 
                      '{exception.Message}'."
                      """
        };

        return context.Response.WriteAsJsonAsync(problem);
    }

}
