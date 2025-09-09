using HorsesForCourses.Core;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var padAPI = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"../../../../HorsesForCourses.WebApi/")); //AppContext.BaseDirectory geeft map waar program wordt uitgevoerd
var dbPath = Path.Combine(padAPI, "horses.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));   //loggen :     options.UseSqlite($"Data Source={dbPath.PulseToLog()}"));

builder.Services.AddScoped<CoachAvailabilityService>();
builder.Services.AddScoped<ICoachRepository, EfCoachRepository>();
builder.Services.AddScoped<ICourseRepository, EfCourseRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

//Cookie auth
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", o => { o.LoginPath = "/Account/Login"; o.LogoutPath = "/Account/Logout"; });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// builder.Services.AddScoped<DomainExceptionFilter>();
// builder.Services.AddControllersWithViews(options =>
// {
//     options.Filters.Add<DomainExceptionFilter>(); // <- use the filter, not the exception type
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
