using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using vaccine.Application.Configurations;
using vaccine.Application.Constants;
using vaccine.Domain;
using vaccine.Domain.Enums;
using vaccine.integration.tests.Fixtures;

namespace vaccine.integration.tests;

public class VaccineApiFactory(InfraFixture infraFixture) : WebApplicationFactory<VaccineApiProgram>
{
    private readonly DatabaseFixture _dbFixture = infraFixture.Database;
    private IOptions<AuthenticationSettings> _options;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });

        builder.ConfigureServices((context, services) =>
        {
            RemoveService(typeof(DbContextOptions<VaccineDbContext>), services);
            
            services.AddDbContext<VaccineDbContext>(options =>
                options.UseNpgsql(_dbFixture.VaccineConnectionString));
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VaccineDbContext>();
            db.Database.Migrate();

            _options = scope.ServiceProvider.GetRequiredService<IOptions<AuthenticationSettings>>();
        });
    }
    
    private static void RemoveService(Type serviceToRemove, IServiceCollection services)
    {
        var ServiceToRemove = services.FirstOrDefault(sv => sv.ServiceType == serviceToRemove);
        if (ServiceToRemove is null)
            throw new InvalidOperationException($"The service with name {serviceToRemove.Name} not found to remove");

        services.Remove(ServiceToRemove);
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, ((int)ERole.Admin).ToString()),
            new Claim(ClaimTypes.Email, "123@email.com"),
            new Claim(VaccineClaimTypes.PersonId, string.Empty),
        };
        
        var expires = DateTime.UtcNow.AddMinutes(90);
        var token = GenerateToken(claims, expires);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
    
    private string GenerateToken(
        IEnumerable<Claim> claims,
        DateTime expires,
        string algorithm = SecurityAlgorithms.HmacSha256)
    {
        var key = new SymmetricSecurityKey(_options.Value.SecretKey);

        var creds = new SigningCredentials(key, algorithm);

        var token = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
