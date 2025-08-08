namespace HorsesForCourses.Core;
/* `StartTime` en `EndTime` zijn van het type `TimeOnly` (vanaf .NET 6), bedoeld om enkel kloktijden (zonder datum) te representeren.
Beide zijn **readonly properties** → je stelt ze alleen in via de constructor. Nu gebruik ik INT */

public record TimeSlot                               //record gemaakt ipv class om duidelijker te maken dat dit een value object is
{
    public int StartTime { get; init; }
    public int EndTime { get; init; }
    public TimeSlot(int startTime, int endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start moet vroeger zijn dan het einde.");

        if (endTime - startTime < 1)
            throw new ArgumentException("Een les moet minstens 1 uur lang zijn.");
        // var duration = endTime - startTime;                 // een lesuur is minstens 1u - enkel nog gebruiken bij TimeOnly
        // if (duration.TotalMinutes < 60)
        // throw new ArgumentException("Een les moet minstens 1 uur lang zijn.");

        if (startTime < 9 || endTime > 17)
            throw new ArgumentException("Een tijdslot moet tijdens de kantooruren vallen (09:00-17:00).");
        // if (timeSlot.StartTime < new TimeOnly(9, 0) || timeSlot.EndTime > new TimeOnly(17, 0))      // Kantooruren: 9u – 17u - bij TimeOnly
        // throw new ArgumentException("Een tijdslot moet tijdens de kantooruren vallen (09:00-17:00).");

        StartTime = startTime;
        EndTime = endTime;
    }

    public TimeSlot() : this(default, default) { }
    public bool OverlapsWith(TimeSlot other)
    {
        return StartTime < other.EndTime && EndTime > other.StartTime;
    }
}