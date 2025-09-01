// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
using HorsesForCourses.Core;
using HorsesForCourses.WebApi;
using HorsesForCourses.Application.common;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.Application;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HorsesForCourses API",
        Version = "v1",
        Description = "API voor het beheren van cursussen en coaches"
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=horses.db"));

builder.Services.AddScoped<CoachAvailabilityService>();
builder.Services.AddScoped<ICourseRepository, EfCourseRepository>();        //Voor elke unieke HTTP-request wordt er één AppDbContext-instantie gemaakt.
builder.Services.AddScoped<ICoachRepository, EfCoachRepository>();
builder.Services.AddScoped<Logger>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

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
