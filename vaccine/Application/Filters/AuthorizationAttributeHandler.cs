using Microsoft.AspNetCore.Authorization;
using vaccine.Application.Configurations;

namespace vaccine.Application.Filters;

public sealed class AuthorizationAttributeHandler : IEndpointFilter
{
    public readonly IRequestInfo _requestInfo;

    public AuthorizationAttributeHandler(IRequestInfo requestInfo)
    {
        _requestInfo = requestInfo;
    }
    
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        
        var roleAttribute = httpContext
            .GetEndpoint()?
            .Metadata
            .GetMetadata<AuthorizationAttributeAnnotation>();

        if (roleAttribute is null)
            return await next(context);

        if (_requestInfo.Role is not null && !roleAttribute.Roles.HasFlag(_requestInfo.Role))
            return await next(context);
        
        return Results.Forbid();
    }
}
