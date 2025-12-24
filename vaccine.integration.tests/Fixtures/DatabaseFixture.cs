using Testcontainers.PostgreSql;

namespace vaccine.integration.tests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    private const string VaccineDatabaseName = "vaccine-test-container-db";

    public string VaccineConnectionString { get; private set; } = null!;
    public PostgreSqlContainer Container { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        
        Container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithName(VaccineDatabaseName)
            .WithPassword("MyStrong!Passw0rd")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_PID", "Express")
            .WithPortBinding(5432, true)
            .Build();

        await Container.StartAsync();
        
        await Container.ExecScriptAsync($"CREATE DATABASE [{VaccineDatabaseName}]");

        var baseConnectionString = Container.GetConnectionString();

        VaccineConnectionString = $"{baseConnectionString};Database={VaccineDatabaseName}";
    }

    public async Task DisposeAsync()
        => await Container.DisposeAsync();
}