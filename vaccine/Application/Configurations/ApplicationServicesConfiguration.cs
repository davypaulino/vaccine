using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using vaccine.Application.Middlewares;
using vaccine.Application.Services;
using vaccine.Domain;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Validators;

namespace vaccine.Application.Configurations;

public static class ApplicationServicesConfiguration
{
    public static TBuilder AddApiDocumentation<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddOpenApi((options) =>
        {
            options.AddDocumentTransformer<OpenApiDocumentationTransform>();
        });

        return builder;
    }

    public static TBuilder AddRequestValidators<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddScoped<IValidator<CreateVaccineRequest>, CreateVaccineRequestValidator>();
        builder.Services.AddScoped<IValidator<ModifyVaccineRequest>, ModifyVaccineRequestValidator>();

        return builder;
    }

    public static TBuilder AddApplicationServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        return builder;
    }
    
    public static TBuilder AddDatabaseService<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddDbContext<VaccineDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
        
        return builder;
    }

    public static TBuilder AddServicesSettings<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddScoped<IRequestInfo, RequestInfo>();

        builder.Services
            .AddOptions<AuthenticationSettings>()
            .Bind(builder.Configuration.GetRequiredSection(nameof(AuthenticationSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }

    public static TBuilder AddMiddlewares<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddScoped<ExceptionMiddleware>();
        builder.Services.AddScoped<RequestInfoMiddleware>();
        
        return builder;
    }

    public static TBuilder AddAuthorization<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddAuthorization();
        return builder;
    }

    public static TBuilder AddAuthentication<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var jwtSettings = builder.Configuration.GetSection(nameof(AuthenticationSettings)).Get<AuthenticationSettings>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var key = jwtSettings.SecretKey;
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        return builder;
    }
}