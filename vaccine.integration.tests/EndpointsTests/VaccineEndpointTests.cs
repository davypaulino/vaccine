using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using vaccine.Data;
using vaccine.Data.Enums;
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
        var context = scope.ServiceProvider.GetRequiredService<VaccineDBContext>();
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
}