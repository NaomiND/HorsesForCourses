namespace HorsesForCourses.Core;

public class ScheduledTimeSlot                      //lesmoment
{
    public WeekDays Day { get; }                    //enum WeekDays
    public TimeSlot TimeSlot { get; }

    public ScheduledTimeSlot(WeekDays day, TimeSlot timeSlot)
    {
        if (!Enum.IsDefined(typeof(WeekDays), day))                                                 // Extra veiligheid indien ooit WeekDays enum wordt uitgebreid
            throw new ArgumentException("Ongeldig: enkel lessen van maandag tot vrijdag toegestaan.");

        if (timeSlot.StartTime < new TimeOnly(9, 0) || timeSlot.EndTime > new TimeOnly(17, 0))      // Kantooruren: 9u – 17u
            throw new ArgumentException("Een tijdslot moet tijdens de kantooruren vallen (09:00-17:00).");

        Day = day;
        TimeSlot = timeSlot;
    }

    public bool OverlapsWith(ScheduledTimeSlot other)
    {
        if (this.Day != other.Day)                              //overlap kan enkel op dezelfde dag
            return false;

        return this.TimeSlot.OverlapsWith(other.TimeSlot);      //overlap in tijdslot checken
    }

    //indien het later nodig zou zijn om tijdsloten met elkaar te vergelijken kan ik Equals + GetHashCode gebruiken en hier toevoegen
}