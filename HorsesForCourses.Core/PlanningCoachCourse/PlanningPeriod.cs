namespace HorsesForCourses.Core;

public class PlanningPeriod                     // werkt zoals Timeslot
{
    public DateOnly StartDate { get; }          // input in dit formaat : yyyy-mm-dd
    public DateOnly EndDate { get; }
    public PlanningPeriod(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date can't be before start date.");

        if (IsWeekend(startDate))
            throw new ArgumentException("Start date can't be in weekends.");

        if (IsWeekend(endDate))
            throw new ArgumentException("End date can't be in weekends.");

        StartDate = startDate;
        EndDate = endDate;
    }

    public bool Contains(DateOnly date)
    {
        return date >= StartDate && date <= EndDate;
    }

    private bool IsWeekend(DateOnly date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }
}