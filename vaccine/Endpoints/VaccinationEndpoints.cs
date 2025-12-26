using vaccine.Application.Constants;

namespace vaccine.Endpoints;

public static class VaccinationEndpoints
{
    private const string CLASSNAME = nameof(VaccinationEndpoints);

    private static string[] _tags =
    [
        ApiDocumentationConstants.VaccinationTag.Name
    ];

    public static RouteGroupBuilder MapvaccinationEndpoints(this IEndpointRouteBuilder app)
    {
        var apiVersion = 1;
        var group = app
            .MapGroup($"/api/v{apiVersion}/vaccinations")
            .WithTags(_tags)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOrder(12);

        return group;
    }
}