using vaccine.Application.Configurations;
using vaccine.Application.Constants;
using vaccine.Application.Filters;
using vaccine.Application.Helpers;
using vaccine.Domain;
using vaccine.Domain.Entities;
using vaccine.Domain.Enums;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Responses;

namespace vaccine.Endpoints;

public class VaccineEndpointsLogger { }

public static class VaccineEndpoints
{
    private const string CLASSNAME = nameof(VaccineEndpoints);
    private static readonly string[] _tags =
    [
        ApiDocumentationConstants.ManagerVaccineTag.Name
    ];

    public static RouteGroupBuilder MapVaccineEndpoints(this IEndpointRouteBuilder app)
    {
        var apiVersion = 1;
        var group = app
            .MapGroup($"/api/v{apiVersion}/vaccines")
            .WithTags(_tags)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOrder(11)
            .RequireAuthorization()
            .AddEndpointFilter<AuthorizationAttributeHandler>();

        group.MapPost("/", CreateVaccine)
            .AddEndpointFilter<ValidationFilter<CreateVaccineRequest>>()
            .Produces<CreateVaccineResponse>(StatusCodes.Status201Created)
            .WithMetadata(new AuthorizationAttributeAnnotation([ERole.Admin, ERole.Editor]))
            .WithSummary("Adiciona Vacina")
            .WithDescription($"""
                              **Responsável por adicionar nova vacina:**
                              {Environment.NewLine}Opções para doses:
                              {EnumDescriptionHelper.GetEnumDescription<EDoseType>()}
                              """);
        
        group.MapPut("/{vaccineId}", ModifyVaccine)
            .AddEndpointFilter<ValidationFilter<ModifyVaccineRequest>>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithMetadata(new AuthorizationAttributeAnnotation([ERole.Admin, ERole.Editor]))
            .WithSummary("Modificar Vacina")
            .WithDescription($"""
                              **Responsável por modificar:**
                              {Environment.NewLine}Opções para doses:
                              {EnumDescriptionHelper.GetEnumDescription<EDoseType>()}
                              """);
        
        return group.WithOpenApi();
    }

    private static async Task<IResult> CreateVaccine(
        CreateVaccineRequest request,
        VaccineDbContext context,
        ILogger<VaccineEndpointsLogger> logger,
        IRequestInfo requestInfo,
        CancellationToken cancellationToken)
    {
        var vaccine = await context
            .Vaccines
            .AddAsync(new Vaccine(request.Name, request.AvailableEDoses), cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        var response = new CreateVaccineResponse(vaccine.Entity.Id);
        
        logger.LogInformation("{Class} | {Method} | {VaccineId} | vaccine created | {CorrelationId}",
            CLASSNAME, nameof(CreateVaccine), response.vaccineId, requestInfo.CorrelationId);
        
        return Results.Created($"/api/v1/vaccines/{vaccine.Entity.Id}", response);
    }

    private static async Task<IResult> ModifyVaccine(
        Guid vaccineId,
        ModifyVaccineRequest request,
        VaccineDbContext context,
        ILogger<VaccineEndpointsLogger> logger,
        IRequestInfo requestInfo,
        CancellationToken cancellation)
    {
        var vaccine = context.Vaccines
            .FirstOrDefault(v => v.Id == vaccineId);

        if (vaccine is null)
        { 
            logger.LogWarning("{Class} | {Method} | {VaccineId} | vaccine not found | {CorrelationId}",
                CLASSNAME, nameof(ModifyVaccine), vaccineId, requestInfo.CorrelationId);
            
            return Results.Problem(
                type: ProblemDetailTypes.NotFound,
                title: "Vaccine Not Found",
                detail: $"Vaccine with id '{vaccineId}' was not found.",
                statusCode: StatusCodes.Status404NotFound,
                extensions: new Dictionary<string, object?>
                {
                    ["vaccineId"] = vaccineId,
                    ["correlationId"] = requestInfo.CorrelationId
                });
        }
        
        vaccine.Update(request.Name, request.AvailableDoses);
        context.Update(vaccine);
        await context.SaveChangesAsync(cancellation);
        
        logger.LogInformation("{Class} | {Method} | {VaccineId} | vaccine updated | {CorrelationId}",
            CLASSNAME, nameof(ModifyVaccine), vaccineId, requestInfo.CorrelationId);
        
        return Results.NoContent();
    }
}