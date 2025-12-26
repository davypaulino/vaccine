using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using vaccine.Application.Constants;

namespace vaccine.Application.Configurations;

public class OpenApiDocumentationTransform : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Tags = ApiDocumentationConstants.Tags;
        
        return Task.CompletedTask;
    }
}