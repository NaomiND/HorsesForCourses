using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HorsesForCourses.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Coach> Coaches { get; set; }
    public DbSet<Course> Courses { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var coachBuilder = modelBuilder.Entity<Coach>();                            //coach mapping
        coachBuilder.HasKey(c => c.Id);
        coachBuilder.Property(c => c.Id).ValueGeneratedOnAdd();

        coachBuilder.OwnsOne(c => c.Name, namebuilder =>                            // Value Object: FullName
        {
            namebuilder.Property(n => n.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(100);
            namebuilder.Property(n => n.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(100);
        });

        coachBuilder.Property(c => c.Email).HasConversion(email => email.Value, value => EmailAddress.From(value)).HasColumnName("Email").IsRequired().HasMaxLength(200);   // EmailAddress als string opslaan
        coachBuilder.HasIndex(c => c.Email).IsUnique();                             // Unieke index op Email

        coachBuilder.Property(c => c.Skills).HasField("skills")                     // Private field: skills, opgeslagen als JSON array
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null) ?? "[]",  // Serialize skills to JSON
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
            );
    }
}

