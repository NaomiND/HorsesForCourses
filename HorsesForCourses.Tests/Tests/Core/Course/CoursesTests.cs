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

    // [Fact]  // herschijven na aanpassing assign coach met timeslots
    // public void AssignCoach_Valid_SetCoachChangeStatusToFinalized()
    // {
    //     var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
    //     var course = Course.Create("C#", period);
    //     course.AddSkill("Unit Testing");
    //     var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12));
    //     course.AddScheduledTimeSlot(slot);
    //     course.Confirm();

    //     var coach = Coach.Create(42, "Coach Test", "test@example.com");
    //     coach.AddSkill("Unit Testing");

    //     course.AssignCoach(coach);

    //     Assert.Equal(CourseStatus.Finalized, course.Status);
    //     Assert.Equal(coach, course.AssignedCoach);
    // }
}