using HorsesForCourses.Core;
namespace HorsesForCourses.Tests.Core.Planning;

public class TimeSlotTests
{
    [Fact]
    public void Constructor_WithValidTimes_CreatesTimeSlot()
    {
        var startTime = 9;
        var endTime = 10;

        var timeSlot = new TimeSlot(startTime, endTime);
        Assert.Equal(startTime, timeSlot.StartTime);
        Assert.Equal(endTime, timeSlot.EndTime);
    }
    [Theory]
    [InlineData(9, 9)]
    [InlineData(10, 9)]
    public void Constructor_WithInvalidTimeOrderOrInvalidDuration_ThrowsArgumentException(int startTime, int endTime)
    {
        var exception = Assert.Throws<ArgumentException>(() => new TimeSlot(startTime, endTime));

        Assert.Equal("Starthour must be before endhour.", exception.Message);
    }

    [Fact]
    public void Constructor_WithDurationLessThanOneHour_ThrowsArgumentException()
    {
        var startTime = 10;
        var endTime = 10;

        var exception = Assert.Throws<ArgumentException>(() => new TimeSlot(startTime, endTime));
        Assert.Equal("Starthour must be before endhour.", exception.Message);
    }

    [Theory]
    [InlineData(8, 10)]
    [InlineData(16, 18)]
    [InlineData(8, 18)]
    [InlineData(17, 18)]

    public void Constructor_WithTimesOutsideOfficeHours_ThrowsArgumentException(int startTime, int endTime)
    {
        var exception = Assert.Throws<ArgumentException>(() => new TimeSlot(startTime, endTime));
        Assert.Equal("A timeslot must be between office hours (09:00-17:00).", exception.Message);
    }

    [Fact]
    public void OverlapsWith_TimeSlotsOverlap_ReturnsTrue()
    {
        var timeSlot1 = new TimeSlot(10, 12);
        var timeSlot2 = new TimeSlot(11, 13);

        var result = timeSlot1.OverlapsWith(timeSlot2);
        Assert.True(result);
    }

    [Fact]
    public void OverlapsWith_TimeSlotsDoNotOverlap_ReturnsFalse()
    {
        var timeSlot1 = new TimeSlot(10, 11);
        var timeSlot2 = new TimeSlot(11, 12);

        var result = timeSlot1.OverlapsWith(timeSlot2);
        Assert.False(result);
    }

    [Fact]
    public void OverlapsWith_OtherTimeSlotIsContainedWithinThisOne_ReturnsTrue()
    {
        var timeSlot1 = new TimeSlot(11, 12);
        var timeSlot2 = new TimeSlot(10, 14);

        var result = timeSlot1.OverlapsWith(timeSlot2);
        Assert.True(result);
    }

    [Fact]
    public void OverlapsWith_TimeSlotsTouchAtEnd_ReturnsFalse()
    {
        var timeSlot1 = new TimeSlot(10, 12);
        var timeSlot2 = new TimeSlot(12, 14);

        var result = timeSlot1.OverlapsWith(timeSlot2);
        Assert.False(result);
    }
}