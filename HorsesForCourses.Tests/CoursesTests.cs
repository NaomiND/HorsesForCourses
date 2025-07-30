using HorsesForCourses.Core;
namespace HorsesForCourses.Tests;

public class CourseTests
{
    [Fact]
    public void CreateValidCourse_WithDraftStatus()
    {
        var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
        var course = Course.Create(0, "C#", period);

        Assert.Equal("C#", course.CourseName);
        Assert.Equal(CourseStatus.Draft, course.Status);
    }

    [Fact]
    public void AddRequiredCompetence_AddsNewToList()
    {
        var id = 42;
        var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
        var course = Course.Create(id, "C#", period);

        course.AddRequiredCompetence("Testing");

        Assert.Contains("testing", course.RequiredCompetencies);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddRequiredCompetency_InvalidInput_ThrowsArgumentException(string invalidCompetency)
    {
        var id = 42;
        var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
        var course = Course.Create(id, "C#", period);

        Assert.Throws<ArgumentException>(() => course.AddRequiredCompetence(invalidCompetency));
    }

    [Fact]
    public void AddCompetence_ThrowsInvalidOperationExceptionWhenCompetenceAlreadyExists()
    {
        var id = 42;
        var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
        var course = Course.Create(id, "C#", period);
        string competence = "Dutch";
        course.AddRequiredCompetence(competence);

        Assert.Throws<InvalidOperationException>(() => course.AddRequiredCompetence("dutch"));
    }

    [Fact]
    public void RemoveRequiredCompetence_RemovesExistingCompetence()
    {
        var id = 42;
        var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
        var course = Course.Create(id, "C#", period);
        course.AddRequiredCompetence("Testing");

        course.RemoveRequiredCompetence("testing");

        Assert.Empty(course.RequiredCompetencies);
    }

    [Fact]
    public void AddScheduledTimeSlot_Valid_AddsSuccessfully()
    {
        var id = 42;
        var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
        var course = Course.Create(id, "C#", period);
        var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(new TimeOnly(10, 0), new TimeOnly(12, 0)));

        course.AddScheduledTimeSlot(slot);

        Assert.Single(course.ScheduledTimeSlots);
    }

    [Fact]
    public void Confirm_ChangesStatusToConfirmed()
    {
        var id = 42;
        var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
        var course = Course.Create(id, "C#", period);
        var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(new TimeOnly(10, 0), new TimeOnly(12, 0)));

        course.AddScheduledTimeSlot(slot);
        course.Confirm();

        Assert.Equal(CourseStatus.Confirmed, course.Status);
    }

    [Fact]
    public void AssignCoach_Valid_SetCoachChangeStatusToFinalized()
    {
        var id = 42;
        var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
        var course = Course.Create(id, "C#", period);
        course.AddRequiredCompetence("Unit Testing");
        var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(new TimeOnly(10, 0), new TimeOnly(12, 0)));
        course.AddScheduledTimeSlot(slot);
        course.Confirm();

        var coach = Coach.Create(42, "Coach Test", "test@example.com");
        coach.AddCompetence("Unit Testing");

        course.AssignCoach(coach);

        Assert.Equal(CourseStatus.Finalized, course.Status);
        Assert.Equal(coach, course.AssignedCoach);
    }
}