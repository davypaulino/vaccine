using Microsoft.EntityFrameworkCore;
using vaccine.Application.Configurations;
using vaccine.Application.Constants;
using vaccine.Application.Filters;
using vaccine.Domain;
using vaccine.Domain.Entities;
using vaccine.Domain.Enums;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Responses;

namespace vaccine.Endpoints;

public class VaccinationEndpointsLogger { }

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
            .RequireAuthorization()
            .WithOrder(12);
        
        group.MapPost("/", CreateVaccination)
            .AddEndpointFilter<ValidationFilter<CreateVaccinationRequest>>()
            .WithMetadata(new AuthorizationAttributeAnnotation([ERole.Admin, ERole.Editor]))
            .Produces<CreateVaccinationResponse>(StatusCodes.Status201Created)
            .WithSummary("Criar vacinação")
            .WithDescription("""
                                 Cria uma nova vacinação para uma pessoa.
                                 - Recebe PersonId e VaccineId
                                 - Inicializa a vacinação sem doses aplicadas
                                 - Retorna o Id da vacinação criada
                             """);

        group.MapDelete("/{personId:guid}/vaccinations/{vaccinationId:guid}", DeleteUserVaccination)
            .Produces(StatusCodes.Status204NoContent)
            .WithMetadata(new AuthorizationAttributeAnnotation([ERole.Admin, ERole.Editor]))
            .WithSummary("Deletar vacinação de uma pessoa")
            .WithDescription("""
                                 Remove uma vacinação específica de uma pessoa.
                                 - Todas as doses associadas serão removidas automaticamente.
                             """);

        return group;
    }
    
    private static async Task<IResult> CreateVaccination(
        CreateVaccinationRequest request,
        VaccineDbContext context,
        ILogger<VaccinationEndpointsLogger> logger,
        IRequestInfo requestInfo,
        CancellationToken cancellationToken)
    {
        var personExists = await context.Persons
            .AnyAsync(p => p.Id == request.PersonId, cancellationToken);
        
        if (!personExists)
            return Results.NotFound($"Person with id '{request.PersonId}' not found.");

        var vaccineExists = await context.Vaccines
            .AnyAsync(v => v.Id == request.VaccineId, cancellationToken);
        
        if (!vaccineExists)
            return Results.NotFound($"Vaccine with id '{request.VaccineId}' not found.");

        var vaccination = await context.Vaccinations
            .Include(v => v.Person) 
            .Include(v => v.Vaccine)
            .Include(v => v.Doses)
            .FirstOrDefaultAsync(v => v.VaccineId == request.VaccineId && v.PersonId == request.PersonId, cancellationToken);

        if (vaccination is null) 
        {
            vaccination = new Vaccination
            {
                PersonId = request.PersonId,
                VaccineId = request.VaccineId,
                Doses = new HashSet<Dose>()
            };

            await context.Vaccinations
                .AddAsync(vaccination, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        var vacc = await context.Vaccinations
            .Include(v => v.Person) 
            .Include(v => v.Vaccine)
            .Include(v => v.Doses)
            .FirstOrDefaultAsync(v => v.VaccineId == request.VaccineId && v.PersonId == request.PersonId, cancellationToken);
        
        var canAdd = request.Doses?
            .All(d =>
                vacc.Vaccine.AvailableTypes
                    .HasFlag(d.DoseType)) ?? true;
         
        if (canAdd is false)
        {
            return Results.Problem(
                type: ProblemDetailTypes.BadRequest,
                title: "Invalid dose",
                detail: $"Vaccine don't had this doses",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?>
                {
                    ["correlationId"] = requestInfo.CorrelationId
                });
        }

        var newDoses = request.Doses
            .Select(d => new Dose(vacc.Id, d.DoseType, d.AppliedAt))
            .ToHashSet();
        
        await context.AddRangeAsync(newDoses, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("{Class} | {Method} | VaccinationId: {VaccinationId} | created | CorrelationId: {CorrelationId}",
            CLASSNAME, nameof(CreateVaccination), vaccination.Id, requestInfo.CorrelationId);

        return Results.Created($"/api/v1/vaccinations/{vaccination.Id}", new CreateVaccinationResponse(vaccination.Id));
    }
    
    private static async Task<IResult> DeleteUserVaccination(
        Guid personId,
        Guid vaccinationId,
        VaccineDbContext context,
        ILogger<VaccinationEndpointsLogger> logger,
        IRequestInfo requestInfo,
        CancellationToken cancellationToken)
    {
        var vaccination = await context.Vaccinations
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == vaccinationId && v.PersonId == personId, cancellationToken);
    
        if (vaccination == null)
        {
            logger.LogWarning("{Class} | {Method} | {PersonId} | {VaccinationId} | not found | {CorrelationId}",
                CLASSNAME, nameof(DeleteUserVaccination), personId, vaccinationId, requestInfo.CorrelationId);
    
            return Results.NotFound($"Vaccination with id '{vaccinationId}' for person '{personId}' not found.");
        }
        
        context.Vaccinations.Remove(vaccination);
        await context.SaveChangesAsync(cancellationToken);
    
        logger.LogInformation("{Class} | {Method} | {PersonId} | {VaccinationId} | deleted | {CorrelationId}",
            CLASSNAME, nameof(DeleteUserVaccination), personId, vaccinationId, requestInfo.CorrelationId);
    
        return Results.NoContent();
    }

}