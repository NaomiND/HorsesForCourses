namespace HorsesForCourses.Core;

public class PlanningPeriod                     // werkt zoals Timeslot
{
    public DateOnly StartDate { get; }  // input in dit formaat : yyyy-mm-dd
    public DateOnly EndDate { get; }
    private PlanningPeriod() { }        // EF Core vereist een parameterloze constructor
    public PlanningPeriod(DateOnly startDate, DateOnly endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("Einddatum moet na startdatum liggen.");

        StartDate = startDate;
        EndDate = endDate;
    }

    public bool Contains(DateOnly date)
    {
        return date >= StartDate && date <= EndDate;
    }
}