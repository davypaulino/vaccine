using Microsoft.EntityFrameworkCore;
using vaccine.Data;
using vaccine.Domain;

namespace vaccine.Application.Configurations;

public static class DatabaseServiceConfiguration
{
    public static TBuilder AddDatabaseService<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddDbContext<VaccineDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
        
        return builder;
    }
}