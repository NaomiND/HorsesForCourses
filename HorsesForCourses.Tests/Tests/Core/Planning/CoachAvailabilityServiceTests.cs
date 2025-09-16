using HorsesForCourses.Application;
using HorsesForCourses.Core;
using Xunit;

public class CoachAvailabilityTests
{
    private readonly CoachAvailability availabilityService;
    private readonly Coach coach1;
    private readonly Coach coach2;

    public CoachAvailabilityTests()
    {
        availabilityService = new CoachAvailability();
        coach1 = Coach.Create("Ine De Wit", "Ine.dewit@gmail.com");
        coach2 = Coach.Create("Test Example", "test.example@test.com");
    }

    [Fact]
    public async Task Coach_With_No_Existing_Courses_Should_Be_Available()
    {
        var newCourse = CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot>
        {
            Slot(WeekDays.Monday, 10, 12)
        });

        var result = await availabilityService.IsCoachAvailableForCourse(coach1, newCourse, new List<Course>());

        Assert.True(result);
    }

    [Fact]
    public async Task Coach_With_Overlapping_Course_And_Overlapping_Timeslot_Should_Not_Be_Available()
    {
        var existingCourse = CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot>
        {
            Slot(WeekDays.Monday, 10, 12)
        });

        var newCourse = CreateCourseForTest(2, coach1, new List<ScheduledTimeSlot>
        {
            Slot(WeekDays.Monday, 11, 13)
        });

        var result = await availabilityService.IsCoachAvailableForCourse(coach1, newCourse, new List<Course> { existingCourse });

        Assert.False(result);
    }

    [Fact]
    public async Task Coach_With_Overlapping_Course_But_NonOverlapping_Timeslot_Should_Be_Available()
    {
        var existingCourse = CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot>
        {
            Slot(WeekDays.Monday, 9, 10)
        });

        var newCourse = CreateCourseForTest(2, coach1, new List<ScheduledTimeSlot>
        {
            Slot(WeekDays.Monday, 10, 11)
        });

        var result = await availabilityService.IsCoachAvailableForCourse(coach1, newCourse, new List<Course> { existingCourse });

        Assert.True(result);
    }

    [Fact]
    public async Task Coach_With_Course_Outside_New_Course_Period_Should_Be_Available()
    {
        var existingCourse = new Course("Oude cursus", new PlanningPeriod(
            new DateOnly(2024, 12, 1), new DateOnly(2024, 12, 20)));
        existingCourse.AssignCoach(coach1);
        existingCourse.AddScheduledTimeSlot(Slot(WeekDays.Monday, 10, 12));
        typeof(Course).GetProperty("Id")!.SetValue(existingCourse, 1);

        var newCourse = CreateCourseForTest(2, coach1, new List<ScheduledTimeSlot>
        {
            Slot(WeekDays.Monday, 10, 12)
        }, new DateOnly(2025, 1, 10), new DateOnly(2025, 1, 30));

        var result = await availabilityService.IsCoachAvailableForCourse(coach1, newCourse, new List<Course> { existingCourse });

        Assert.True(result);
    }

    [Fact]
    public async Task Same_Course_Id_Should_Be_Ignored_In_Availability_Check()
    {
        var course = CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot>
        {
            Slot(WeekDays.Wednesday, 13, 15)
        });

        var result = await availabilityService.IsCoachAvailableForCourse(coach1, course, new List<Course> { course });

        Assert.True(result);
    }

    // -----------------------------------------------
    // 🔧 Helper Methods
    // -----------------------------------------------

    private ScheduledTimeSlot Slot(WeekDays day, int startHour, int endHour)
    {
        return new ScheduledTimeSlot(day, new TimeSlot(startHour, endHour));
    }

    private Course CreateCourseForTest(
        int id,
        Coach coach,
        List<ScheduledTimeSlot> slots,
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        var course = new Course(
            "Test Cursus",
            new PlanningPeriod(
                startDate ?? new DateOnly(2025, 1, 1),
                endDate ?? new DateOnly(2025, 1, 31)));

        typeof(Course).GetProperty("Id")!.SetValue(course, id);
        course.AssignCoach(coach);

        foreach (var slot in slots)
            course.AddScheduledTimeSlot(slot);

        return course;
    }
}

















// public class CoachAvailabilityTests
// {
//     private readonly CoachAvailability availabilityService;
//     private readonly Coach coach1;
//     private readonly Coach coach2;

//     public CoachAvailabilityTests()              // Initialisatie voor elke test
//     {
//         availabilityService = new CoachAvailability(ICoachRepository, courseRepository);
//         coach1 = new Coach(FullName.From("Ine De Wit"), EmailAddress.Create("Ine.dewit@gmail.com"));
//         coach2 = new Coach(FullName.From("Test Example"), EmailAddress.Create("test.example@test.com"));
//     }

//     private Course CreateCourseForTest(int id, Coach coach, List<ScheduledTimeSlot> slots)
//     {
//         var course = new Course("Test Course", new PlanningPeriod(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)));

//         typeof(Course).GetProperty("Id").SetValue(course, id);
//         typeof(Course).GetProperty("AssignedCoach").SetValue(course, coach);

//         // De ScheduledTimeSlots lijst is private, dus we moeten een work-around vinden.
//         // Een betere benadering is om een AddScheduledTimeSlot methode te gebruiken.
//         foreach (var slot in slots)
//         {
//             course.AddScheduledTimeSlot(slot);
//         }
//         return course;
//     }

//     [Fact]
//     public void IsCoachAvailableForCourse_NoExistingCourses_ReturnsTrue()
//     {
//         var newCourse = CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot> {
//             new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12))
//         });
//         var allCourses = new List<Course>();

//         var result = availabilityService.IsCoachAvailableForCourse(coach1, newCourse);

//         Assert.True(result);
//     }

//     [Fact]
//     public void IsCoachAvailableForCourse_WithNonOverlappingCourses_ReturnsTrue()
//     {
//         var newCourse = CreateCourseForTest(10, coach1, new List<ScheduledTimeSlot> {
//             new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(14, 16))
//         });
//         var existingCourses = new List<Course> {
//             CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12)) }),
//             CreateCourseForTest(2, coach1, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Tuesday, new TimeSlot(10, 12)) }),
//             CreateCourseForTest(3, coach2, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12)) }) // Een andere coach
//         };

//         var result = availabilityService.IsCoachAvailableForCourse(coach1, newCourse, existingCourses);

//         Assert.True(result);
//     }

//     [Fact]
//     public void IsCoachAvailableForCourse_WithTimeOverlap_ReturnsFalse()
//     {
//         var newCourse = CreateCourseForTest(10, coach1, new List<ScheduledTimeSlot> {
//             new ScheduledTimeSlot(WeekDays.Wednesday, new TimeSlot(11, 13))
//         });
//         var existingCourses = new List<Course> {
//             CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Wednesday, new TimeSlot(10, 12)) })
//         };

//         var result = availabilityService.IsCoachAvailableForCourse(coach1, newCourse, existingCourses);

//         Assert.False(result);
//     }

//     [Fact]
//     public void IsCoachAvailableForCourse_WithMultipleTimeSlotsAndTimeOverlap_ReturnsFalse()
//     {
//         var newCourse = CreateCourseForTest(10, coach1, new List<ScheduledTimeSlot> {
//             new ScheduledTimeSlot(WeekDays.Wednesday, new TimeSlot(10, 12)),
//             new ScheduledTimeSlot(WeekDays.Thursday, new TimeSlot(14, 16))
//         });
//         var existingCourses = new List<Course> {
//             CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Thursday, new TimeSlot(15, 17)) })
//         };

//         var result = availabilityService.IsCoachAvailableForCourse(coach1, newCourse, existingCourses);

//         Assert.False(result);
//     }

//     [Fact]
//     public void IsCoachAvailableForCourse_WithDifferentDay_ReturnsTrue()
//     {
//         var newCourse = CreateCourseForTest(10, coach1, new List<ScheduledTimeSlot> {
//             new ScheduledTimeSlot(WeekDays.Tuesday, new TimeSlot(10, 12))
//         });
//         var existingCourses = new List<Course> {
//             CreateCourseForTest(1, coach1, new List<ScheduledTimeSlot> { new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12)) })
//         };

//         var result = availabilityService.IsCoachAvailableForCourse(coach1, newCourse, existingCourses);

//         Assert.True(result);
//     }
// }