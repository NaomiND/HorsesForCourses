using HorsesForCourses.Core;

public class PlanningPeriodTests
{
    [Fact]
    public void Constructor_WithValidDates_CreatesPlanningPeriod()
    {
        var startDate = new DateOnly(2025, 1, 1);
        var endDate = new DateOnly(2025, 1, 31);

        var planningPeriod = new PlanningPeriod(startDate, endDate);

        Assert.Equal(startDate, planningPeriod.StartDate);
        Assert.Equal(endDate, planningPeriod.EndDate);
    }

    [Fact]
    public void Constructor_WithEqualDates_CreatesPlanningPeriod()
    {
        var date = new DateOnly(2025, 1, 31);

        var planningPeriod = new PlanningPeriod(date, date);

        Assert.Equal(date, planningPeriod.StartDate);
        Assert.Equal(date, planningPeriod.EndDate);
    }

    [Fact]
    public void Constructor_WithEndDateBeforeStartDate_ThrowsArgumentException()
    {
        var startDate = new DateOnly(2025, 2, 1);
        var endDate = new DateOnly(2025, 1, 30);

        var exception = Assert.Throws<ArgumentException>(() => new PlanningPeriod(startDate, endDate));
        Assert.Equal("Einddatum mag niet voor de startdatum liggen.", exception.Message);
    }

    [Fact]
    public void Contains_DateWithinPeriod_ReturnsTrue()
    {
        var planningPeriod = new PlanningPeriod(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 30));
        var dateToCheck = new DateOnly(2025, 6, 15);

        var result = planningPeriod.Contains(dateToCheck);

        Assert.True(result);
    }

    [Fact]
    public void Contains_DateBeforePeriod_ReturnsFalse()
    {
        var planningPeriod = new PlanningPeriod(new DateOnly(2025, 9, 1), new DateOnly(2025, 9, 30));
        var dateToCheck = new DateOnly(2025, 8, 31);

        var result = planningPeriod.Contains(dateToCheck);

        Assert.False(result);
    }
}