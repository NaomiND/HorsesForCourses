using System.Reflection;
using HorsesForCourses.Core;
namespace HorsesForCourses.Tests;

public class CourseTests
{
    private readonly PlanningPeriod _defaultPeriod;

    public CourseTests()
    {
        _defaultPeriod = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
    }

    [Fact]
    public void Course_Constructor_InitializesPropertiesCorrectly()
    {
        var courseName = "Introduction to Testing";
        var period = new PlanningPeriod(new DateOnly(2025, 10, 1), new DateOnly(2025, 10, 31));

        var course = new Course(courseName, period);

        Assert.Equal(courseName, course.Name);
        Assert.Equal(period, course.Period);
        Assert.Equal(CourseStatus.Draft, course.Status);
        Assert.Empty(course.ScheduledTimeSlots);
        Assert.Empty(course.CourseSkills);
        Assert.Null(course.AssignedCoach);
    }

    [Fact]
    public void Create_ValidCourse_SetsNamePeriodAndDraftStatus()
    {
        var course = Course.Create("C#", _defaultPeriod);

        Assert.Equal("C#", course.Name);
        Assert.Equal(_defaultPeriod, course.Period);
        Assert.Equal(CourseStatus.Draft, course.Status);
    }

    [Fact]
    public void AddScheduledTimeSlot_Valid_AddsSuccessfully()
    {
        var course = Course.Create("C#", _defaultPeriod);
        var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12));
        // var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(new TimeOnly(10, 0), new TimeOnly(12, 0))); // Voor TimeOnly, maar nu met int start en end time

        course.AddScheduledTimeSlot(slot);

        Assert.Single(course.ScheduledTimeSlots);
        Assert.Contains(slot, course.ScheduledTimeSlots);
    }

    [Fact]
    public void Confirm_ChangesStatusToConfirmed()
    {
        var course = Course.Create("C#", _defaultPeriod);
        var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12));

        course.AddScheduledTimeSlot(slot);
        course.Confirm();

        Assert.Equal(CourseStatus.Confirmed, course.Status);
    }

    [Fact]
    public void AddScheduledTimeSlot_WhenStatusIsNotDraft_ThrowsInvalidOperationException()
    {
        var course = Course.Create("C# Basics", _defaultPeriod);
        var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12));
        course.AddScheduledTimeSlot(slot);
        course.Confirm();

        var exception = Assert.Throws<InvalidOperationException>(() => course.AddScheduledTimeSlot(slot));
        Assert.Equal("Timeslot can't be changed after confirmation or coach assignment.", exception.Message);
    }

    [Fact]
    public void Confirm_WhenCourseHasTimeSlots_ChangesStatusToConfirmed()
    {
        var course = Course.Create("C# Basics", _defaultPeriod);
        var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12));
        course.AddScheduledTimeSlot(slot);

        course.Confirm();

        Assert.Equal(CourseStatus.Confirmed, course.Status);
    }

    [Fact]
    public void Confirm_WhenCourseHasNoTimeSlots_ThrowsInvalidOperationException()
    {
        var course = Course.Create("C# Basics", _defaultPeriod);

        var exception = Assert.Throws<InvalidOperationException>(() => course.Confirm());
        Assert.Equal("Confirmation failed. Please add timeslot(s).", exception.Message);
    }

    [Fact]
    public void AssignCoach_Valid_SetCoachChangeStatusToFinalized()
    {
        var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
        var course = Course.Create("C#", period);

        var requiredSkill = new Skill("unit testing");
        var courseSkill = new CourseSkill { Course = course, Skill = requiredSkill };

        // Use reflection to add the skill to the private collection
        var courseSkillsField = typeof(Course).GetField("_courseSkills", BindingFlags.NonPublic | BindingFlags.Instance);
        (courseSkillsField.GetValue(course) as List<CourseSkill>).Add(courseSkill);

        var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12));
        course.AddScheduledTimeSlot(slot);
        course.Confirm();

        var coach = Coach.Create("Coach Test", "test@example.com");
        var coachSkill = new CoachSkill { Coach = coach, Skill = requiredSkill };

        var coachSkillsField = typeof(Coach).GetField("coachSkills", BindingFlags.NonPublic | BindingFlags.Instance);
        (coachSkillsField.GetValue(coach) as List<CoachSkill>).Add(coachSkill);

        course.AssignCoach(coach);

        Assert.Equal(CourseStatus.Finalized, course.Status);
        Assert.Equal(coach, course.AssignedCoach);
    }
}