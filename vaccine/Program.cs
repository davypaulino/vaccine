using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using vaccine.Endpoints;
using vaccine.Application.Configurations;
using vaccine.Application.Middlewares;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection(nameof(AuthenticationSettings)));
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

builder.Services.AddAuthorization();

builder.AddDatabaseService();
builder.Services.AddScoped<IRequestInfo, RequestInfo>();
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddScoped<RequestInfoMiddleware>();
builder.Services.AddScoped<IValidator<CreateVaccineRequest>, CreateVaccineRequestValidator>();
builder.Services.AddScoped<IValidator<ModifyVaccineRequest>, ModifyVaccineRequestValidator>();
builder.Services.AddOpenApi((options) =>
{
    options.AddDocumentTransformer<OpenApiDocumentationTransform>();
});

var app = builder.Build();

app.UseMiddleware<RequestInfoMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api-reference", options =>
    {
        options
            .WithTitle("Vaccine API")
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.Http, ScalarClient.Curl);
    });
}

app.UseHttpsRedirection();
app.MapVaccineEndpoints();
await app.RunAsync();

public partial class VaccineApiProgram { }