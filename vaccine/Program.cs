using FluentValidation;
using Scalar.AspNetCore;
using vaccine.Endpoints;
using vaccine.Application.Configurations;
using vaccine.Application.Middlewares;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabaseService();
builder.Services.AddScoped<IRequestInfo, RequestInfo>();
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddScoped<RequestInfoMiddleware>();
builder.Services.AddScoped<IValidator<CreateVaccineRequest>, CreateVaccineRequestValidator>();
builder.Services.AddScoped<IValidator<ModifyVaccineRequest>, ModifyVaccineRequestValidator>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<RequestInfoMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api-reference", options =>
    {
        options
            .WithTitle("Vaccine API")
            .WithTheme(ScalarTheme.Default)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.MapVaccineEndpoints();
await app.RunAsync();

public partial class VaccineApiProgram { }