using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

public class AppDbContextTests : DbContextTestBase
{
    [Fact]
    public async Task Coach_CanBeSavedAndRetrieved_PropertiesAreCorrect()
    {
        var id = 0;
        var fullName = FullName.From("Jan De Tester");
        var email = EmailAddress.Create("jan.tester@example.com");
        var newCoach = new Coach(fullName, email);
        newCoach.AddSkill("C#");
        newCoach.AddSkill("Testing");

        // Act
        _context.Coaches.Add(newCoach);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear(); // Maak de context leeg om zeker te zijn dat we data uit de DB halen

        var savedCoach = await _context.Coaches.FindAsync(newCoach.Id);

        // Assert
        Assert.NotNull(savedCoach);
        Assert.Equal(newCoach.Id, savedCoach.Id);
        Assert.Equal("Jan De Tester", savedCoach.Name.DisplayName);
        Assert.Equal("jan.tester@example.com", savedCoach.Email.Value);
        Assert.Contains("c#", savedCoach.Skills); // Skills worden lowercase opgeslagen
        Assert.Contains("testing", savedCoach.Skills);
    }

    [Fact]
    public async Task Coach_SavingDuplicateEmail_ThrowsDbUpdateException()
    {
        // Arrange
        var email = EmailAddress.Create("duplicate@example.com");
        var coach1 = new Coach(FullName.From("Coach Een"), email);
        var coach2 = new Coach(FullName.From("Coach Twee"), email);

        _context.Coaches.Add(coach1);
        await _context.SaveChangesAsync();

        // Act & Assert
        _context.Coaches.Add(coach2);

        // De unieke index op Email moet een exception veroorzaken
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
    }

    // [Fact]
    // public async Task Course_CanBeSavedAndRetrieved_WithOwnedTypesAndRelations()
    // {
    //     // Arrange
    //     var period = new PlanningPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 6, 30));
    //     var newCourse = Course.Create("Advanced EF Core", period);

    //     // Voeg owned collection items (ScheduledTimeSlot) toe
    //     newCourse.AddScheduledTimeSlot(new ScheduledTimeSlot(
    //         WeekDays.Monday, new TimeSlot(new TimeOnly(9, 0), new TimeOnly(12, 0))
    //     ));
    //     newCourse.AddSkill("EF Core");
    //     newCourse.Confirm();

    //     // Maak en koppel een gerelateerde entiteit (Coach)
    //     //var coach = new Coach(FullName.From("Elia Vercruysse"), EmailAddress.From("elia.v@example.com"));
    //     var fullName = FullName.From("Jan De Tester");
    //     var email = EmailAddress.From("jan.tester@example.com");
    //     var newCoach = new Coach(fullName, email);
    //     newCoach.AddSkill("EF Core");
    //     newCourse.AssignCoach(newCoach);

    //     // Act
    //     _context.Courses.Add(newCourse); // EF Core detecteert automatisch de nieuwe coach ook
    //     await _context.SaveChangesAsync();
    //     _context.ChangeTracker.Clear();

    //     var savedCourse = await _context.Courses
    //         .Include(c => c.AssignedCoach) // Vergeet niet gerelateerde data expliciet te laden
    //         .FirstOrDefaultAsync(c => c.Id == newCourse.Id);

    //     // Assert
    //     Assert.NotNull(savedCourse);
    //     Assert.Equal("Advanced EF Core", savedCourse.Name);
    //     Assert.Equal(CourseStatus.Finalized, savedCourse.Status);

    //     // Assert Owned Type (PlanningPeriod)
    //     Assert.Equal(new DateOnly(2025, 1, 1), savedCourse.Period.StartDate);

    //     // Assert Owned Collection (ScheduledTimeSlots)
    //     Assert.Single(savedCourse.ScheduledTimeSlots);
    //     var timeSlot = savedCourse.ScheduledTimeSlots.First();
    //     Assert.Equal(WeekDays.Monday, timeSlot.Day);
    //     Assert.Equal(new TimeOnly(9, 0), timeSlot.TimeSlot.StartTime);

    //     // Assert Relation (AssignedCoach)
    //     Assert.NotNull(savedCourse.AssignedCoach);
    //     Assert.Equal("Jan De Tester", savedCourse.AssignedCoach.Name.DisplayName);
    // }

    [Fact]
    public async Task Course_SavingWithoutName_ThrowsDbUpdateException()
    {
        // Arrange
        var period = new PlanningPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 6, 30));
        var course = new Course(null!, period); // Forceer een null-waarde die door de DB geweigerd moet worden

        // Act & Assert
        _context.Courses.Add(course);

        // De `IsRequired()` constraint op de Name-property moet een exception veroorzaken
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
    }
}