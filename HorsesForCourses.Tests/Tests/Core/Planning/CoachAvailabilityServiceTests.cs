using HorsesForCourses.Core;

public class CoachAvailabilityServiceTests
{
    private readonly CoachAvailabilityService availabilityService;
    private readonly Coach coach1;
    private readonly Coach coach2;

    public CoachAvailabilityServiceTests()              // Initialisatie voor elke test
    {
        availabilityService = new CoachAvailabilityService();
        coach1 = new Coach(FullName.From("Ine De Wit"), EmailAddress.Create("Ine.dewit@gmail.com"));
        coach2 = new Coach(FullName.From("Test Example"), EmailAddress.Create("test.example@test.com"));
    }

    private Course CreateCourseForTest(int id, Coach coach, List<ScheduledTimeSlot> slots)
    {
        var course = new Course("Test Course", new PlanningPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)));

        typeof(Course).GetProperty("Id").SetValue(course, id);
        typeof(Course).GetProperty("AssignedCoach").SetValue(course, coach);

        // De ScheduledTimeSlots lijst is private, dus we moeten een work-around vinden.
        // Een betere benadering is om een AddScheduledTimeSlot methode te gebruiken.
        foreach (var slot in slots)
        {
            course.AddScheduledTimeSlot(slot);
        }
        return course;
    }

    [Fact]
    public void IsCoachAvailableForCourse_NoExistingCourses_ReturnsTrue()
    {
        var newCourse = CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot> {
            new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12))
        });
        var allCourses = new List<Course>();

        var result = availabilityService.IsCoachAvailableForCourse(coach1, newCourse, allCourses);

        Assert.True(result);
    }

    [Fact]
    public void IsCoachAvailableForCourse_WithNonOverlappingCourses_ReturnsTrue()
    {
        var newCourse = CreateCourseForTest(10, coach1, new List<ScheduledTimeSlot> {
            new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(14, 16))
        });
        var existingCourses = new List<Course> {
            CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12)) }),
            CreateCourseForTest(2, coach1, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Tuesday, new TimeSlot(10, 12)) }),
            CreateCourseForTest(3, coach2, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12)) }) // Een andere coach
        };

        var result = availabilityService.IsCoachAvailableForCourse(coach1, newCourse, existingCourses);

        Assert.True(result);
    }

    [Fact]
    public void IsCoachAvailableForCourse_WithTimeOverlap_ReturnsFalse()
    {
        var newCourse = CreateCourseForTest(10, coach1, new List<ScheduledTimeSlot> {
            new ScheduledTimeSlot(WeekDays.Wednesday, new TimeSlot(11, 13))
        });
        var existingCourses = new List<Course> {
            CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Wednesday, new TimeSlot(10, 12)) })
        };

        var result = availabilityService.IsCoachAvailableForCourse(coach1, newCourse, existingCourses);

        Assert.False(result);
    }

    [Fact]
    public void IsCoachAvailableForCourse_WithMultipleTimeSlotsAndTimeOverlap_ReturnsFalse()
    {
        var newCourse = CreateCourseForTest(10, coach1, new List<ScheduledTimeSlot> {
            new ScheduledTimeSlot(WeekDays.Wednesday, new TimeSlot(10, 12)),
            new ScheduledTimeSlot(WeekDays.Thursday, new TimeSlot(14, 16))
        });
        var existingCourses = new List<Course> {
            CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Thursday, new TimeSlot(15, 17)) })
        };

        var result = availabilityService.IsCoachAvailableForCourse(coach1, newCourse, existingCourses);

        Assert.False(result);
    }

    [Fact]
    public void IsCoachAvailableForCourse_WithDifferentDay_ReturnsTrue()
    {
        var newCourse = CreateCourseForTest(10, coach1, new List<ScheduledTimeSlot> {
            new ScheduledTimeSlot(WeekDays.Tuesday, new TimeSlot(10, 12))
        });
        var existingCourses = new List<Course> {
            CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12)) })
        };

        var result = availabilityService.IsCoachAvailableForCourse(coach1, newCourse, existingCourses);

        Assert.True(result);
    }
}