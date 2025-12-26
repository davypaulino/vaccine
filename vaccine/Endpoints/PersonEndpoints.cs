using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vaccine.Application.Configurations;
using vaccine.Application.Constants;
using vaccine.Application.Filters;
using vaccine.Domain;
using vaccine.Domain.Entities;
using vaccine.Domain.Enums;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Responses;
using vaccine.Endpoints.DTOs.Validators;

namespace vaccine.Endpoints;

public class PersonEndpointsLogger { }

public static class PersonEndpoints
{
    private const string CLASSNAME = nameof(VaccinationEndpoints);

    private static string[] _tags =
    [
        ApiDocumentationConstants.PersonTag.Name
    ];

    public static RouteGroupBuilder MapPersonEndpoints(this IEndpointRouteBuilder app)
    {
        var apiVersion = 1;
        var group = app
            .MapGroup($"/api/v{apiVersion}/persons")
            .WithTags(_tags)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOrder(13);

        group.MapPost("/", CreatePerson)
            .Produces<CreatePersonResponse>(StatusCodes.Status201Created)
            .AddEndpointFilter<ValidationFilter<CreatePersonRequest>>()
            .WithSummary("Criar pessoa")
            .WithDescription("""
                             Cria um novo registo de pessoa no sistema.

                             Este endpoint é responsável por:
                             - Validar os dados da pessoa
                             - Criar o registo de pessoa
                             - Retornar os dados da pessoa criada
                             """);

        group.MapDelete("/{personId:guid}", DeletePerson)
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Deletar pessoa")
            .WithDescription("""
                             Remove uma pessoa do sistema.

                             Ao excluir uma pessoa:
                             - Todas as vacinações associadas serão removidas
                             - Todas as doses relacionadas também serão excluídas
                             """);

        group.MapGet("/", GetPersonsPaged)
            .Produces<PagedResponse<PersonResponse>>(StatusCodes.Status200OK)
            .WithSummary("Listar pessoas (paginado)")
            .WithDescription("""
                             Retorna uma lista paginada de pessoas cadastradas no sistema.
                             """);

        group.MapGet("/{personId:guid}/vaccinations", GetPersonVaccinations)
            .Produces<IEnumerable<VaccinationResponse>>(StatusCodes.Status200OK)
            .WithSummary("Listar vacinações de uma pessoa")
            .WithDescription("""
                             Retorna todas as vacinações associadas a uma pessoa específica.
                             """);

        return group;
    }

    private static async Task<IResult> CreatePerson(
        CreatePersonRequest request,
        VaccineDbContext context,
        ILogger<PersonEndpointsLogger> logger,
        IRequestInfo requestInfo,
        CancellationToken cancellationToken)
    {
        var document = new Cpf(request.Document);
        var exist = await context.Persons
            .AnyAsync(p => p.Document == document, cancellationToken);

        if (exist)
        {
            logger.LogWarning(
                "{Class} | {Method} | {UserDocument} | {AuthenticatedUserId} | user already exists | {CorrelationId}",
                CLASSNAME, nameof(CreatePerson), document.Number, requestInfo.UserId, requestInfo.CorrelationId);

            return Results.Problem(
                type: ProblemDetailTypes.BadRequest,
                title: "Can't create Person",
                detail: $"Can't create Person.",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?>
                {
                    ["document"] = document.Number,
                    ["correlationId"] = requestInfo.CorrelationId
                });
        }

        var person = await context.Persons.AddAsync(
            new Person(
                request.Name,
                document,
                request.BirthDate),
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        var response = new CreatePersonResponse(person.Entity.Id);

        logger.LogInformation(
            "{Class} | {Method} | {AuthenticatedUserId} | {PersonId} | person created | {CorrelationId}",
            CLASSNAME, nameof(CreatePerson), requestInfo.UserId, response.PersonId, requestInfo.CorrelationId);

        return Results.Created($"/api/v1/persons/{person.Entity.Id}", response);
    }

    private static async Task<IResult> DeletePerson(
        Guid personId,
        VaccineDbContext context,
        ILogger<PersonEndpointsLogger> logger,
        IRequestInfo requestInfo,
        CancellationToken cancellationToken)
    {
        var person = context.Persons.FirstOrDefault(p => p.Id == personId);

        if (person is null)
        {
            logger.LogWarning(
                "{Class} | {Method} | {AuthenticatedUserId} | {PersonId} | person not found | {CorrelationId}",
                CLASSNAME, nameof(DeletePerson), requestInfo.UserId, personId, requestInfo.CorrelationId);

            return Results.Problem(
                type: ProblemDetailTypes.NotFound,
                title: "Person Not Found",
                detail: $"Person with id '{personId}' was not found.",
                statusCode: StatusCodes.Status404NotFound,
                extensions: new Dictionary<string, object?>
                {
                    ["personId"] = personId,
                    ["correlationId"] = requestInfo.CorrelationId
                });
        }

        context.Persons.Remove(person);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("{Class} | {Method} | {PersonId} | person deleted | {CorrelationId}",
            CLASSNAME, nameof(DeletePerson), personId, requestInfo.CorrelationId);

        return Results.NoContent();
    }

    private static async Task<IResult> GetPersonsPaged(
        VaccineDbContext context,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        
        var query = context.Persons.AsNoTracking();

        var total = await query.CountAsync(cancellationToken);

        var persons = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PersonResponse(
                p.Id,
                p.Name,
                p.Document.Number,
                p.Birthday))
            .ToListAsync(cancellationToken);

        return Results.Ok(new
        {
            page,
            pageSize,
            total,
            data = persons
        });
    }
    
    private static async Task<IResult> GetPersonVaccinations(
        Guid personId,
        VaccineDbContext context,
        ILogger<PersonEndpointsLogger> logger,
        IRequestInfo requestInfo,
        CancellationToken cancellationToken)
    {
        var person = await context.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == personId, cancellationToken);

        if (person is null)
        {
            logger.LogWarning("{Class} | {Method} | {PersonId} | person not found | {CorrelationId}",
                CLASSNAME, nameof(GetPersonVaccinations), personId, requestInfo.CorrelationId);

            return Results.Problem(
                type: ProblemDetailTypes.NotFound,
                title: "Person Not Found",
                detail: $"Person with id '{personId}' was not found.",
                statusCode: StatusCodes.Status404NotFound,
                extensions: new Dictionary<string, object?>
                {
                    ["personId"] = personId,
                    ["correlationId"] = requestInfo.CorrelationId
                });
        }
        
        var vaccinations = await context.Vaccinations
            .AsNoTracking()
            .Where(v => v.PersonId == personId)
            .Select(v => new VaccinationResponse
                {
                    VaccineName = v.Vaccine.Name,
                    AvailableDoses = v.Vaccine.AvailableTypes,
                    Doses = v.Doses.Select(d => new DoseResponse
                    {
                        DoseType = d.DoseType,
                        AppliedAt = d.AppliedAt
                    }).ToHashSet()
                }
            ).ToHashSetAsync(cancellationToken);

        var noVaccinations = await context.Vaccines
            .AsNoTracking()
            .ToHashSetAsync(cancellationToken);

        foreach (var v in  noVaccinations)
        {
            vaccinations.Add(
                new VaccinationResponse
                {
                    VaccineName = v.Name,
                    AvailableDoses = v.AvailableTypes,
                    Doses = []
                }
            );
        }
        
        var age = DateTime.UtcNow.Year - person.Birthday.Year;
        if (person.Birthday.Date > DateTime.UtcNow.AddYears(-age)) age--;
        
        var response = new PersonVaccinationsDetailedResponse
        {
            Name = person.Name,
            Document = person.Document.Number,
            Age = age,
            Vaccinations =vaccinations,
        };

        return Results.Ok(response);
    }

}