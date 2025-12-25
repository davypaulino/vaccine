using Microsoft.EntityFrameworkCore;
using vaccine.integration.tests.Helpers;
using vaccine.Data;

namespace vaccine.integration.tests.Fixtures;

public class InfraFixture : IAsyncLifetime
{
    public DatabaseFixture Database { get; } = new();

    public async Task InitializeAsync()
    {
        if (!await DockerHelper.IsDockerRunningAsync())
            throw new InvalidOperationException("Docker is not running!");

        await Database.InitializeAsync();

        await ApplyMigrationsAsync();
    }

    private async Task ApplyMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<VaccineDBContext>()
            .UseNpgsql(Database.VaccineConnectionString)
            .Options;

        using var context = new VaccineDBContext(options);
        await context.Database.MigrateAsync();
    }
    
    public async Task DisposeAsync() => await Database.DisposeAsync();
}