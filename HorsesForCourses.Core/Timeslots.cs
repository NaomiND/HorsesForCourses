namespace HorsesForCourses.Core;

public class TimeSlot
{
    /* `StartTime` en `EndTime` zijn van het type `TimeOnly` (vanaf .NET 6), bedoeld om enkel kloktijden (zonder datum) te representeren.
    Beide zijn **readonly properties** → je stelt ze alleen in via de constructor. */
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }

    public TimeSlot(TimeOnly startTime, TimeOnly endTime)
    {
        if (startTime >= endTime)                           // er moet een start en eind uur zijn
            throw new ArgumentException("Start moet vroeger zijn dan het einde.");

        var duration = endTime - startTime;                 // een lesuur is minstens 1u
        if (duration.TotalMinutes < 60)
            throw new ArgumentException("Een les moet minstens 1 uur lang zijn.");

        StartTime = startTime;
        EndTime = endTime;
    }

    public bool OverlapsWith(TimeSlot other)                // overlapdetectie
    {
        return StartTime < other.EndTime && EndTime > other.StartTime;
    }
}