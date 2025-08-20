using HorsesForCourses.Core;
using HorsesForCourses.Application;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
namespace HorsesForCourses.Tests;

public class CoachPersistancyTests
{
    [Fact]
    public async Task ShouldPersistData()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        using (var context = new AppDbContext(options))
        {
            context.Coaches.Add(new Coach(FullName.From("Test Tester"), EmailAddress.From("email@test.com")));
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var coach = await context.Coaches.FindAsync(1);

            Assert.NotNull(coach);
            Assert.Equal("Test Tester", coach.Name.DisplayName);
            Assert.Equal("email@test.com", coach.Email.Value);
        }
    }
}