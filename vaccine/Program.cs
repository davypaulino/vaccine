using Scalar.AspNetCore;
using vaccine.Endpoints;
using vaccine.Application.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabaseService();
builder.Services.AddOpenApi();

var app = builder.Build();

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
app.RunAsync();

public partial class VaccineApiProgram { }