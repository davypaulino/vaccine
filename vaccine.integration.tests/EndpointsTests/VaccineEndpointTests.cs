using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using vaccine.Domain;
using vaccine.Domain.Entities;
using vaccine.Domain.Enums;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Responses;
using vaccine.integration.tests.Colletctions;
using vaccine.integration.tests.Fixtures;

namespace vaccine.integration.tests.EndpointsTests;

[Collection(nameof(VaccineCollection))]
public class VaccineEndpointTests
{
    private const string endpointPath = "api/v1/vaccines";
    
    private readonly VaccineApiFactory _factory;

    public VaccineEndpointTests(InfraFixture infraFixture)
    {
        _factory = new VaccineApiFactory(infraFixture);
    }

    [Fact]
    public async Task PostVaccine_WithValidRequest_ShouldCreateVaccine()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VaccineDbContext>();
        await context.Vaccines.AddRangeAsync(context.Vaccines!);
        var client = _factory.CreateClient();
        
        var request =
            new CreateVaccineRequest("COVID-19", [EDoseType.First, EDoseType.Second, EDoseType.FirstReinforcement]);
        
        // Act
        var response = await client.PostAsJsonAsync($"{endpointPath}", request);

        // Assert
        var content = await response.Content.ReadFromJsonAsync<CreateVaccineResponse>();

        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content!.vaccineId);
        
        Assert.NotNull(context.Vaccines.FirstOrDefault(v => v.Id == content!.vaccineId));
    }
    
    [Fact]
    public async Task PostVaccine_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new CreateVaccineRequest(
            "",
            [EDoseType.First]
        );

        // Act
        var response = await client.PostAsJsonAsync(endpointPath, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.True(problem!.Errors.ContainsKey("Name"));
    }

    [Fact]
    public async Task PostVaccine_WithoutDoses_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new CreateVaccineRequest(
            "COVID-19",
            Array.Empty<EDoseType>()
        );

        // Act
        var response = await client.PostAsJsonAsync(endpointPath, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.True(problem!.Errors.ContainsKey("AvailableEDoses"));
    }

    [Fact]
    public async Task PostVaccine_WithInvalidEnumValue_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            Name = "COVID-19",
            AvailableEDoses = new[] { 999 }
        };

        // Act
        var response = await client.PostAsJsonAsync(endpointPath, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.True(problem!.Errors.ContainsKey("AvailableEDoses[0]"));
    }

    [Fact]
    public async Task PostVaccine_ShouldPersistCorrectValuesAndReturn201()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VaccineDbContext>();
        await context.Vaccines.AddRangeAsync(context.Vaccines!);
        var client = _factory.CreateClient();

        var request = new CreateVaccineRequest(
            "Influenza",
            [EDoseType.First, EDoseType.Second]
        );

        // Act
        var response = await client.PostAsJsonAsync(endpointPath, request);
        var content = await response.Content
            .ReadFromJsonAsync<CreateVaccineResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var vaccine = context.Vaccines
            .FirstOrDefault(v => v.Id == content!.vaccineId);

        Assert.NotNull(vaccine);
        Assert.Equal("Influenza", vaccine!.Name);
        Assert.Equal(
            EDoseType.First | EDoseType.Second,
            vaccine.AvailableTypes
        );
    }

    [Fact]
    public async Task PutVaccine_WithValidRequest_ShouldUpdateVaccine()
    {
        // Arrange
        await CleanDatabase(CancellationToken.None);
        const string expectedName = "COVID-19 Updated";
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VaccineDbContext>();
        await context.Vaccines.AddRangeAsync(context.Vaccines!);
        var client = _factory.CreateClient();

        var vaccine = new Vaccine(
            "COVID-19",
            [EDoseType.First, EDoseType.Second]);

        context.Vaccines.Add(vaccine);
        await context.SaveChangesAsync();

        var request = new ModifyVaccineRequest(
            expectedName,
            [EDoseType.First, EDoseType.Second, EDoseType.FirstReinforcement]);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{endpointPath}/{vaccine.Id}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var verifyScope = _factory.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider.GetRequiredService<VaccineDbContext>();

        var updatedVaccine = await verifyContext.Vaccines.FirstOrDefaultAsync(v => v.Id == vaccine.Id);

        Assert.Equal(expectedName, updatedVaccine.Name);
        Assert.True(updatedVaccine.AvailableTypes.HasFlag(EDoseType.FirstReinforcement));
    }

    [Fact]
    public async Task PutVaccine_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var fakeId = Guid.NewGuid();

        var request = new ModifyVaccineRequest(
            "Any name",
            [EDoseType.First]);

        // Act
        var response = await client.PutAsJsonAsync(
            $"{endpointPath}/{fakeId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutVaccine_WithInvalidEnumValue_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new ModifyVaccineRequest("COVID-19", [(EDoseType)999]);

        // Act
        var response = await client.PutAsJsonAsync($"{endpointPath}/{Guid.NewGuid()}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.True(problem!.Errors.ContainsKey("AvailableDoses[0]"));
    }

    [Fact]
    public async Task PutVaccine_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new ModifyVaccineRequest("", [EDoseType.First]);

        // Act
        var response = await client.PutAsJsonAsync($"{endpointPath}/{Guid.NewGuid()}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.True(problem!.Errors.ContainsKey("Name"));
    }

    [Fact]
    public async Task PutVaccine_WithoutDoses_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new ModifyVaccineRequest("COVID-19", Array.Empty<EDoseType>());

        // Act
        var response = await client.PutAsJsonAsync($"{endpointPath}/{Guid.NewGuid()}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.True(problem!.Errors.ContainsKey("AvailableDoses"));
    }

    private async Task CleanDatabase(CancellationToken cancellationToken)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VaccineDbContext>();
        context.Vaccines.RemoveRange(context.Vaccines!);
        context.Persons.RemoveRange(context.Persons!);
        context.Users.RemoveRange(context.Users!);
        await context.SaveChangesAsync(cancellationToken);
    }
}