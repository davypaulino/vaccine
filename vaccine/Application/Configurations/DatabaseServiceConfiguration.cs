using Microsoft.EntityFrameworkCore;
using vaccine.Data;

namespace vaccine.Application.Configurations;

public static class DatabaseServiceConfiguration
{
    public static TBuilder AddDatabaseService<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddDbContext<VaccineDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
        
        return builder;
    }
}