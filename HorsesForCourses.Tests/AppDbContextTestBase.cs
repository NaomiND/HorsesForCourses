using HorsesForCourses.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

// IDisposable zorgt ervoor dat de resources (databaseverbinding) na elke test netjes worden opgeruimd.
public abstract class DbContextTestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly AppDbContext _context;

    protected DbContextTestBase()
    {
        // De connectiestring ":memory:" creëert een tijdelijke in-memory database.
        // De verbinding moet manueel geopend worden en open blijven zolang de DbContext leeft.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);

        // Zorgt ervoor dat het databaseschema (tabellen, etc.) wordt aangemaakt
        // op basis van de OnModelCreating-configuratie.
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}