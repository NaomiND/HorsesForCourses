using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests.Application;

public class CoursePersistenceTests : DbContextTestBase
{
    [Fact]
    public async Task Course_CanBeSavedAndRetrieved_WithSkillsAndTimeslots()
    {
        var skill = new Skill("ef core");
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        var period = new PlanningPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 6, 30));
        var newCourse = Course.Create("Advanced EF Core", period);
        newCourse.AddScheduledTimeSlot(new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(9, 12)));

        var courseSkill = new CourseSkill { Course = newCourse, Skill = skill };
        _context.CourseSkills.Add(courseSkill);

        newCourse.Confirm();

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var savedCourse = await _context.Courses
            .Include(c => c.CourseSkills)
            .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == newCourse.Id);

        Assert.NotNull(savedCourse);
        Assert.Equal("Advanced EF Core", savedCourse.Name);
        Assert.Equal(CourseStatus.Confirmed, savedCourse.Status);

        Assert.Equal(new DateOnly(2025, 1, 1), savedCourse.Period.StartDate);
        Assert.Single(savedCourse.ScheduledTimeSlots);
        Assert.Equal(WeekDays.Monday, savedCourse.ScheduledTimeSlots.First().Day);

        Assert.Null(savedCourse.AssignedCoach);

        Assert.Single(savedCourse.CourseSkills);
        Assert.Equal("ef core", savedCourse.CourseSkills.First().Skill.Name);
    }

    [Fact]
    public async Task Course_SavingWithoutName_ThrowsDbUpdateException()
    {
        // Arrange
        var period = new PlanningPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 6, 30));
        var course = new Course(null!, period);

        _context.Courses.Add(course);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
    }
    // [Fact]   == voor ASSIGN COACH
    // public async Task Course_CanBeSavedAndRetrieved_WithOwnedTypesAndRelations()
    // {
    //     //Maak de Coach en Skill die we gaan koppelen (als assign in orde is)
    //     var skill = new Skill("ef core");
    //     _context.Skills.Add(skill);

    //     var coach = Coach.Create("Elia Vercruysse", "elia.v@example.com");
    //     var coachSkill = new CoachSkill { Coach = coach, Skill = skill };
    //     _context.Coaches.Add(coach);
    //     _context.CoachSkills.Add(coachSkill);
    //     await _context.SaveChangesAsync();

    //     //Maak de Course
    //     var period = new PlanningPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 6, 30));
    //     var newCourse = Course.Create("Advanced EF Core", period);
    //     newCourse.AddScheduledTimeSlot(new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(9, 12)));

    //     //Koppel de Skill aan de Course
    //     var courseSkill = new CourseSkill { Course = newCourse, Skill = skill };
    //     _context.CourseSkills.Add(courseSkill);

    //     //Simuleer het domeinproces: bevestig en wijs coach toe
    //     newCourse.Confirm();

    //     // In een echte service-laag zou je de coach toewijzen via een methode die de status ook op Finalized zet.
    //     // Voor een persistentietest is het ok om de property direct te setten.
    //     var courseToUpdate = await _context.Courses.FindAsync(newCourse.Id); // Eerst opslaan om een Id te krijgen
    //     if (courseToUpdate is not null)
    //     {
    //         typeof(Course).GetProperty("AssignedCoach")!.SetValue(courseToUpdate, coach);
    //         typeof(Course).GetProperty("Status")!.SetValue(courseToUpdate, CourseStatus.Finalized);
    //     }

    //     await _context.SaveChangesAsync();
    //     _context.ChangeTracker.Clear();

    //     var savedCourse = await _context.Courses
    //         // .Include(c => c.AssignedCoach)
    //         .Include(c => c.CourseSkills)
    //         .ThenInclude(cs => cs.Skill)
    //         .FirstOrDefaultAsync(c => c.Id == newCourse.Id);

    //     Assert.NotNull(savedCourse);
    //     Assert.Equal("Advanced EF Core", savedCourse.Name);
    //     Assert.Equal(CourseStatus.Finalized, savedCourse.Status);

    //     Assert.Equal(new DateOnly(2025, 1, 1), savedCourse.Period.StartDate);

    //     Assert.Single(savedCourse.ScheduledTimeSlots);
    //     var timeSlot = savedCourse.ScheduledTimeSlots.First();
    //     Assert.Equal(WeekDays.Monday, timeSlot.Day);
    //     Assert.Equal(9, timeSlot.TimeSlot.StartTime);

    //     Assert Relation(AssignedCoach)
    //     Assert.NotNull(savedCourse.AssignedCoach);
    //     Assert.Equal("Elia Vercruysse", savedCourse.AssignedCoach.Name.DisplayName);

    //     // Assert Relation (Skills)
    //     Assert.Single(savedCourse.CourseSkills);
    //     Assert.Equal("ef core", savedCourse.CourseSkills.First().Skill.Name);
    // }

    [Fact]
    public async Task Course_SavingWithoutName_ThrowsInvalidOperationException()
    {
        // Arrange
        // We proberen een Course direct via de constructor aan te maken met null,
        // wat de domeinregel omzeilt maar de database-constraint test.
        var period = new PlanningPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 6, 30));
        var course = new Course(null!, period);

        _context.Courses.Add(course);

        // De `IsRequired()` constraint op de Name-property moet een exception veroorzaken.
        // Omdat de property non-nullable is, zal dit een InvalidOperationException zijn.
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
    }
}