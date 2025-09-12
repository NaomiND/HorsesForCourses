// using HorsesForCourses.Core;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.ChangeTracking;
// using System.Text.Json;

// namespace HorsesForCourses.Application;

// public class AppDbContext : DbContext
// {
//     public DbSet<Coach> Coaches { get; set; }
//     public DbSet<Course> Courses { get; set; }
//     public DbSet<User> Users { get; set; }

//     public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         base.OnModelCreating(modelBuilder);

//         var coachBuilder = modelBuilder.Entity<Coach>();                                //coach mapping
//         coachBuilder.HasKey(c => c.Id);
//         coachBuilder.Property(c => c.Id).ValueGeneratedOnAdd();

//         coachBuilder.OwnsOne(c => c.Name, namebuilder =>                                // Value Object: FullName
//         {
//             namebuilder.Property(n => n.FirstName).HasColumnName("Firstname").IsRequired().HasMaxLength(50);
//             namebuilder.Property(n => n.LastName).HasColumnName("Lastname").IsRequired().HasMaxLength(50);
//         });

//         coachBuilder.Property(c => c.Email)
//                     .HasConversion(email => email.Value, value => EmailAddress.Create(value))   //E-mail als string opslaan
//                     .HasColumnName("Email")
//                     .IsRequired()
//                     .HasMaxLength(100);
//         coachBuilder.HasIndex(c => c.Email)
//                     .IsUnique();                                                        // Unieke index op Email

//         coachBuilder
//             .Property<List<string>>("skills")
//             .UsePropertyAccessMode(PropertyAccessMode.Field)
//             .HasConversion(
//                         v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
//                         v => string.IsNullOrWhiteSpace(v)
//                         ? new List<string>()
//                         : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
//             )
//             .Metadata.SetValueComparer(new ValueComparer<List<string>>(
//                 (c1, c2) => c1.SequenceEqual(c2),
//                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
//                 c => c.ToList()
//             ));

//         var courseBuilder = modelBuilder.Entity<Course>();                          //course mapping
//         courseBuilder.HasKey(c => c.Id);
//         courseBuilder.Property(c => c.Id).ValueGeneratedOnAdd();

//         courseBuilder.Property(c => c.Name).HasColumnName("Cursus").IsRequired().HasMaxLength(150);         //value object: Name

//         courseBuilder.Property(c => c.Status).HasConversion<string>();              // Enum als string opslaan

//         courseBuilder.OwnsOne(c => c.Period, periodBuilder =>                       // Value Object: PlanningPeriod
//         {
//             periodBuilder.Property(p => p.StartDate).HasColumnName("Start date");
//             periodBuilder.Property(p => p.EndDate).HasColumnName("End date");
//         });

//         courseBuilder.Property<List<string>>("skills")                                      // Private field: skills, opgeslagen als JSON array
//             .UsePropertyAccessMode(PropertyAccessMode.Field)
//             .HasConversion(
//                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
//                 v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
//             )
//                      .Metadata.SetValueComparer(new ValueComparer<List<string>>(
//                 (c1, c2) => c1.SequenceEqual(c2),
//                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
//                 c => c.ToList()
//             ));

//         courseBuilder.HasOne(c => c.AssignedCoach)                                  // Relatie: Een cursus heeft één (optionele) coach
//                      .WithMany(c => c.Courses)                                                    // Een Coach heeft geen collectie van Courses, dus dit is eenrichtingsverkeer
//                      .HasForeignKey("AssignedCoachId")
//                      .IsRequired(false);                                            // Een coach is niet verplicht bij aanmaak

//         // courseBuilder.Property(typeof(List<ScheduledTimeSlot>), "scheduledTimeSlots")
//         //              .HasField("scheduledTimeSlots");

//         //     courseBuilder.Property(c => c.ScheduledTimeSlots)
//         //         .HasConversion(
//         //             v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
//         //             v => v == null
//         //                 ? new List<ScheduledTimeSlot>()
//         //                 : JsonSerializer.Deserialize<List<ScheduledTimeSlot>>(v, (JsonSerializerOptions)null) ?? new List<ScheduledTimeSlot>()
//         //         );
//         // }

//         courseBuilder.OwnsMany(c => c.ScheduledTimeSlots, ts =>
//             {
//                 ts.HasKey("Id");
//                 ts.WithOwner().HasForeignKey("CourseId");

//                 ts.Property(t => t.Day)
//                     .HasConversion(
//                         v => v.ToString(),
//                         v => Enum.Parse<WeekDays>(v))
//                     .HasColumnName("Day");
//                 ts.OwnsOne(a => a.TimeSlot, timeSlotBuilder =>
//                 {
//                     timeSlotBuilder.Property(t => t.StartTime).IsRequired();
//                     timeSlotBuilder.Property(t => t.EndTime).IsRequired();
//                 });


//                 // ts.Property(t => t.StartTime);//.HasColumnName("Start");
//                 // ts.Property(t => t.EndTime);//.HasColumnName("End");
//             });
//         // courseBuilder
//         // .OwnsMany(c => c.ScheduledTimeSlots, scheduledTimeSlotBuilder =>
//         //     courseBuilder.Property<List<ScheduledTimeSlot>>("scheduledTimeSlots")                       // Private field: scheduledTimeSlots, opgeslagen als JSON array
//         //                                                                                                 // .Property<List<ScheduledTimeSlot>>("scheduledTimeSlots")                                // Private field: skills, opgeslagen als JSON array
//         //     .UsePropertyAccessMode(PropertyAccessMode.Field)
//         //     .HasColumnName("Time")
//         //     .HasConversion(
//         //         v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
//         //         v => JsonSerializer.Deserialize<List<ScheduledTimeSlot>>(v, (JsonSerializerOptions)null) ?? new List<ScheduledTimeSlot>()
//         //     )
//         // .Metadata.SetValueComparer(new ValueComparer<List<ScheduledTimeSlot>>(
//         // (c1, c2) => c1.SequenceEqual(c2),
//         //     c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
//         //     c => c.ToList()
//         // )));

//         var userBuilder = modelBuilder.Entity<User>();
//         coachBuilder.HasKey(u => u.Id);
//         coachBuilder.Property(u => u.Id).ValueGeneratedOnAdd();

//         userBuilder.OwnsOne(u => u.Name, nameBuilder =>
//         {
//             nameBuilder.Property(n => n.FirstName).HasColumnName("Firstname").IsRequired().HasMaxLength(50);
//             nameBuilder.Property(n => n.LastName).HasColumnName("Lastname").IsRequired().HasMaxLength(50);
//         });

//         userBuilder.Property(u => u.Email)
//         .HasConversion(email => email.Value, value => EmailAddress.Create(value))
//         .HasColumnName("Email")
//         .IsRequired()
//         .HasMaxLength(100);

//         userBuilder.HasIndex(u => u.Email)
//         .IsUnique();

//         userBuilder.Property(u => u.PasswordHash)
//                     .IsRequired();

//         // Definieert de één-op-één relatie tussen User en Coach
//         userBuilder.HasOne(u => u.Coach) // Een User heeft optioneel één Coach
//             .WithOne(c => c.User)        // Een Coach heeft optioneel één User
//             .HasForeignKey<Coach>(c => c.UserId); // De Foreign Key 'UserId' staat op de Coach tabel
//     }
// }