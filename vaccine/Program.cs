using Scalar.AspNetCore;
using vaccine.Application.Configurations;
using vaccine.Application.Middlewares;
using vaccine.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection(nameof(AuthenticationSettings)));

builder.AddAuthentication();
builder.Services.AddAuthorization();
builder.AddServicesSettings();
builder.AddMiddlewares();
builder.AddDatabaseService();
builder.AddRequestValidators();
builder.AddApplicationServices();
builder.AddApiDocumentation();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestInfoMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

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