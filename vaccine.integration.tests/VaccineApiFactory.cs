using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using vaccine.Data;
using vaccine.Domain;
using vaccine.integration.tests.Fixtures;

namespace vaccine.integration.tests;

public class VaccineApiFactory(InfraFixture infraFixture) : WebApplicationFactory<VaccineApiProgram>
{
    private readonly DatabaseFixture _dbFixture = infraFixture.Database;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.Sources.Clear();

            var testSettings = new Dictionary<string, string?>
            {
                ["Logging:LogLevel:Default"] = "Debug",
                ["ConnectionStrings:Database"] = _dbFixture.VaccineConnectionString,
            };

            configBuilder.AddInMemoryCollection(testSettings);
        });

        builder.ConfigureServices((context, services) =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<VaccineDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<VaccineDbContext>(options =>
                options.UseNpgsql(_dbFixture.VaccineConnectionString));
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VaccineDbContext>();
            db.Database.Migrate();
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client.DefaultRequestHeaders.Add("x-api-key", "xy0huehue42");
    }
}
