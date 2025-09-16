using HorsesForCourses.Core;
namespace HorsesForCourses.Tests.Core.Planning;

public class ScheduledTimeSlotTests
{
    [Fact]
    public void Constructor_WithValidDayAndTimeSlot_CreatesScheduledTimeSlot()
    {
        var day = WeekDays.Monday;
        var timeSlot = new TimeSlot(10, 12);

        var scheduledTimeSlot = new ScheduledTimeSlot(day, timeSlot);

        Assert.Equal(day, scheduledTimeSlot.Day);
        Assert.Equal(timeSlot, scheduledTimeSlot.TimeSlot);
    }

    [Fact]
    public void OverlapsWith_SameDayAndOverlappingTime_ReturnsTrue()
    {
        var timeSlot1 = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12));
        var timeSlot2 = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(11, 13));

        var result = timeSlot1.OverlapsWith(timeSlot2);

        Assert.True(result);
    }

    [Fact]
    public void OverlapsWith_SameDayButNonOverlappingTime_ReturnsFalse()
    {
        var timeSlot1 = new ScheduledTimeSlot(WeekDays.Tuesday, new TimeSlot(10, 12));
        var timeSlot2 = new ScheduledTimeSlot(WeekDays.Tuesday, new TimeSlot(12, 14));

        var result = timeSlot1.OverlapsWith(timeSlot2);

        Assert.False(result);
    }

    [Fact]
    public void OverlapsWith_DifferentDays_ReturnsFalse()
    {
        var timeSlot1 = new ScheduledTimeSlot(WeekDays.Wednesday, new TimeSlot(10, 12));
        var timeSlot2 = new ScheduledTimeSlot(WeekDays.Thursday, new TimeSlot(10, 13));

        var result = timeSlot1.OverlapsWith(timeSlot2);

        Assert.False(result);
    }
}