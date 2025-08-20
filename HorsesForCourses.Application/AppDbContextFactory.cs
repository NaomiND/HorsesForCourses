using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HorsesForCourses.Application;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=../HorsesForCourses.WebApi/horses.db"); // Or your connection string

        return new AppDbContext(optionsBuilder.Options);
    }
}

// public AppDbContext CreateDbContext(string[] args)
//     {
//         var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

//         var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");
//         optionsBuilder.UseSqlite($"Data Source={dbPath}"); // Or your connection string

//         return new AppDbContext(optionsBuilder.Options);
//     }

//uitleg : AppCOntext.BaseDirectory: geeft path van waar program.cs gerund wordt.daar voeg je dan app.db aan toe(of horses.db)