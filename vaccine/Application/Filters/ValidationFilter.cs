using vaccine.Application.Configurations;

namespace vaccine.Application.Filters;

using FluentValidation;
using Microsoft.AspNetCore.Http;

public sealed class ValidationFilter<T> : IEndpointFilter
{
    private readonly ILogger<ValidationFilter<T>> _logger;
    private readonly IRequestInfo _requestInfo;

    public ValidationFilter(ILogger<ValidationFilter<T>> logger, IRequestInfo requestInfo)
    {
        _logger = logger;
        _requestInfo = requestInfo;
    }
    
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices
            .GetService<IValidator<T>>();

        if (validator is null)
            return await next(context);

        var model = context.Arguments.OfType<T>().First();

        var result = await validator.ValidateAsync(model);

        if (!result.IsValid)
        {
            _logger.LogWarning(
                "Validation failed for {RequestType} | Errors: {@Errors} | {CorrelationId}",
                typeof(T).Name,
                result.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                }), _requestInfo.CorrelationId);
            
            return Results.ValidationProblem(
                result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            );
        }

        return await next(context);
    }
}
