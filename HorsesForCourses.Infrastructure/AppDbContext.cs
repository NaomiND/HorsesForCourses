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
            namebuilder.Property(n => n.FirstName).HasColumnName("Firstname").IsRequired().HasMaxLength(100);
            namebuilder.Property(n => n.LastName).HasColumnName("Lastname").IsRequired().HasMaxLength(100);
        });

        coachBuilder.Property(c => c.Email).HasConversion(email => email.Value, value => EmailAddress.From(value)).HasColumnName("Email").IsRequired().HasMaxLength(200);   // EmailAddress als string opslaan
        coachBuilder.HasIndex(c => c.Email).IsUnique();                             // Unieke index op Email

        coachBuilder.Property(c => c.Skills).HasField("skills")                     // Private field: skills, opgeslagen als JSON array
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null) ?? "[]",  // Serialize skills to JSON
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
            );

        var courseBuilder = modelBuilder.Entity<Course>();                          //course mapping
        courseBuilder.HasKey(c => c.Id);
        courseBuilder.Property(c => c.Id).ValueGeneratedOnAdd();

        courseBuilder.Property(c => c.Name).HasColumnName("Cursus").IsRequired().HasMaxLength(150);         //value object: Name

        courseBuilder.Property(c => c.Status).HasConversion<string>();              // Enum als string opslaan

        courseBuilder.OwnsOne(c => c.Period, periodBuilder =>                       // Value Object: PlanningPeriod
        {
            periodBuilder.Property(p => p.StartDate).HasColumnName("Start date");
            periodBuilder.Property(p => p.EndDate).HasColumnName("End date");
        });

        courseBuilder.Property(c => c.Skills)                                       // Private field: skills, opgeslagen als JSON array
            .HasField("skills")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
            );
        // courseBuilder.Property(typeof(List<string>), "skills").HasField("skills");

        courseBuilder.HasOne(c => c.AssignedCoach)                                  // Relatie: Een cursus heeft één (optionele) coach
                     .WithMany() // Een Coach heeft geen collectie van Courses, dus dit is eenrichtingsverkeer
                     .HasForeignKey("AssignedCoachId")
                     .IsRequired(false);                                            // Een coach is niet verplicht bij aanmaak

        // courseBuilder.Property(typeof(List<ScheduledTimeSlot>), "scheduledTimeSlots")
        //              .HasField("scheduledTimeSlots");

        courseBuilder.OwnsMany(c => c.ScheduledTimeSlots, slotBuilder =>            // Owned Collection: ScheduledTimeSlots (wordt een aparte tabel)
                    {
                        slotBuilder.WithOwner().HasForeignKey("CourseId");
                        slotBuilder.Property(s => s.Day).HasConversion<string>();
                        slotBuilder.OwnsOne(s => s.TimeSlot, timeBuilder =>         // TimeSlot is ook een Value Object, binnen een ander value object.
                        {
                            timeBuilder.Property(t => t.StartTime).HasColumnName("Start time");
                            timeBuilder.Property(t => t.EndTime).HasColumnName("End time");
                        });
                    });
    }
}
