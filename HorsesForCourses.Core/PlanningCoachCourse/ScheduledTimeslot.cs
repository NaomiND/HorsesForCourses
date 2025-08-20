namespace HorsesForCourses.Core;

public record ScheduledTimeSlot                             //lesmoment (record gemaakt ipv class)
{
    public WeekDays Day { get; init; }                      //enum WeekDays    
    public TimeSlot TimeSlot { get; init; }
    public ScheduledTimeSlot(WeekDays day, TimeSlot timeSlot)
    {
        if (!Enum.IsDefined(typeof(WeekDays), day))
            throw new ArgumentException("Ongeldig: enkel lessen van maandag tot vrijdag toegestaan.");

        Day = day;
        TimeSlot = timeSlot;
    }

    public ScheduledTimeSlot() { }       //private const voor EF/ parameterloze constructor

    public bool OverlapsWith(ScheduledTimeSlot other)
    {
        if (this.Day != other.Day)                              //overlap kan enkel op dezelfde dag
            return false;

        return this.TimeSlot.OverlapsWith(other.TimeSlot);      //overlap in tijdslot checken
    }
}