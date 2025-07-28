// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
using HorsesForCourses.WebApi;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using HorsesForCourses.Repository;
using HorsesForCourses.WebApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "horses-for-courses API",
        Version = "v1",
        Description = "API voor het beheren van cursussen en coaches"
    });
});

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSingleton<InMemoryCourseRepository>();
builder.Services.AddSingleton<InMemoryCoachRepository>();
builder.Services.AddScoped<Logger>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DomainException ex)
    {
        context.Response.StatusCode = 422;
        context.Response.ContentType = "application/problem+json";
        var problem = new ProblemDetails
        {
            Status = 422,
            Title = "Domain Error",
            Detail = ex.Message,
            Type = "https://httpstatuses.com/422"
        };
        await context.Response.WriteAsJsonAsync(problem);
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
