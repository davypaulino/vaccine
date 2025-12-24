using vaccine.Data;
using vaccine.Data.Entities;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Responses;

namespace vaccine.Endpoints;

public static class VaccineEndpoints
{
    private static readonly string[] Tags =
    [
        "vacinas"
    ];

    public static RouteGroupBuilder MapVaccineEndpoints(this IEndpointRouteBuilder app)
    {
        var apiVersion = 1;
        var group = app.MapGroup($"/api/v{apiVersion}/vaccines")
            .WithOpenApi();

        group.MapPost("/", CreateVaccines)
            .WithSummary("Adiciona Vacina")
            .WithDescription("Responsável por adicionar nova vacina.");
        
        return group;
    }

    private static async Task<IResult> CreateVaccines(
        CreateVaccineRequest request,
        VaccineDBContext context)
    {
        var vaccine = await context
            .Vaccines
            .AddAsync(new Vaccine(request.Name, request.AvailableEDoses));
        await context.SaveChangesAsync();
        
        var response = new CreateVaccineResponse(vaccine.Entity.Id);
        
        return Results.Created($"/api/v1/vaccines/{vaccine.Entity.Id}", response);
    }
}