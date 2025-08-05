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

/*// --- Course Configuratie ---
        var courseBuilder = modelBuilder.Entity<Course>();
        courseBuilder.HasKey(c => c.Id);
        courseBuilder.Property(c => c.Id).ValueGeneratedOnAdd();

        courseBuilder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        
        // Enum als string opslaan
        courseBuilder.Property(c => c.Status).HasConversion<string>();

        // Value Object: PlanningPeriod
        courseBuilder.OwnsOne(c => c.Period, periodBuilder =>
        {
            periodBuilder.Property(p => p.StartDate).HasColumnName("Period_StartDate");
            periodBuilder.Property(p => p.EndDate).HasColumnName("Period_EndDate");
        });
        
        // Relatie: Een cursus heeft één (optionele) coach
        courseBuilder.HasOne(c => c.AssignedCoach)
                     .WithMany() // Een Coach heeft geen collectie van Courses, dus dit is eenrichtingsverkeer
                     .HasForeignKey("AssignedCoachId")
                     .IsRequired(false); // Een coach is niet verplicht bij aanmaak

        // Owned Collection: ScheduledTimeSlots (wordt een aparte tabel)
        courseBuilder.OwnsMany(c => c.ScheduledTimeSlots, slotBuilder =>
        {
            slotBuilder.WithOwner().HasForeignKey("CourseId");
            slotBuilder.Property(s => s.Day).HasConversion<string>();
            // TimeSlot is ook een Value Object, binnen een ander value object.
            slotBuilder.OwnsOne(s => s.TimeSlot, timeBuilder =>
            {
                timeBuilder.Property(t => t.StartTime).HasColumnName("StartTime");
                timeBuilder.Property(t => t.EndTime).HasColumnName("EndTime");
            });
        });

        // Private field: _skills, net als bij Coach
        courseBuilder.Property(c => c.Skills)
            .HasField("skills")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
            );
    }*/

/*
 // Course mapping
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Status);

            entity.OwnsOne(c => c.Period, p =>
            {
                p.Property(pp => pp.StartDate).HasColumnName("StartDate");
                p.Property(pp => pp.EndDate).HasColumnName("EndDate");
            });

            entity
                .Property(typeof(List<string>), "skills")
                .HasField("skills");

            entity
                .Property(typeof(List<ScheduledTimeSlot>), "scheduledTimeSlots")
                .HasField("scheduledTimeSlots");

            entity.HasOne(c => c.AssignedCoach);

            // Je kan eventueel ScheduledTimeSlot als owned type instellen
        });
*/