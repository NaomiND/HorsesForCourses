using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

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
        var period = new PlanningPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 6, 30));
        var course = new Course(null!, period);

        _context.Courses.Add(course);

        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
    }


    [Fact]
    public async Task Course_CanBeSavedAndRetrieved_WithOwnedTypesAndRelations()
    {
        var skill = new Skill("ef core");
        var coach = Coach.Create("Ine De Wit", "ine.de.wit@example.com");
        var coachSkill = new CoachSkill { CoachId = coach.Id, SkillId = skill.Id };
        var period = new PlanningPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 6, 30));
        var newCourse = Course.Create("Advanced EF Core", period);
        newCourse.AddScheduledTimeSlot(new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(9, 12)));

        _context.Skills.Add(skill);
        _context.Coaches.Add(coach);
        _context.Courses.Add(newCourse);

        _context.CoachSkills.Add(new CoachSkill { Coach = coach, Skill = skill });
        _context.CourseSkills.Add(new CourseSkill { Course = newCourse, Skill = skill });

        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();
        var courseToUpdate = await _context.Courses.Include(c => c.CourseSkills).ThenInclude(cs => cs.Skill).SingleAsync(c => c.Id == newCourse.Id);
        var coachToAssign = await _context.Coaches.Include(c => c.CoachSkills).ThenInclude(cs => cs.Skill).FirstAsync(c => c.Id == coach.Id);

        Assert.NotNull(courseToUpdate);
        Assert.NotNull(coachToAssign);

        courseToUpdate.Confirm();
        courseToUpdate.AssignCoach(coachToAssign);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var savedCourse = await _context.Courses
            .Include(c => c.AssignedCoach)
            .Include(c => c.CourseSkills)
            .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == newCourse.Id);

        Assert.NotNull(savedCourse);
        Assert.Equal("Advanced EF Core", savedCourse.Name);
        Assert.Equal(CourseStatus.Finalized, savedCourse.Status);
        Assert.Single(savedCourse.ScheduledTimeSlots);
        Assert.Equal(WeekDays.Monday, savedCourse.ScheduledTimeSlots.First().Day);
        Assert.Equal(new DateOnly(2025, 1, 1), savedCourse.Period.StartDate);

        var timeSlot = savedCourse.ScheduledTimeSlots.First();
        Assert.Equal(WeekDays.Monday, timeSlot.Day);
        Assert.Equal(9, timeSlot.TimeSlot.StartTime);

        Assert.NotNull(savedCourse.AssignedCoach);
        Assert.Equal("Ine De Wit", savedCourse.AssignedCoach.Name.DisplayName);

        Assert.Single(savedCourse.CourseSkills);
        Assert.Equal("ef core", savedCourse.CourseSkills.First().Skill.Name);
    }
}