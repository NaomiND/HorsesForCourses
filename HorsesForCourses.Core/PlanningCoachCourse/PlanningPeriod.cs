namespace HorsesForCourses.Core;

public class PlanningPeriod                     // werkt zoals Timeslot
{
    public DateOnly StartDate { get; }          // input in dit formaat : yyyy-mm-dd
    public DateOnly EndDate { get; }
    public PlanningPeriod(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("Einddatum mag niet voor de startdatum liggen.");

        if (IsWeekend(startDate))
            throw new ArgumentException("Startdatum mag niet in het weekend vallen.");

        if (IsWeekend(endDate))
            throw new ArgumentException("Einddatum mag niet in het weekend vallen.");

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