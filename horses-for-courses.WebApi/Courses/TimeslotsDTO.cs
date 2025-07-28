using horses_for_courses.Core;
namespace horses_for_courses.Dtos;

public class ScheduledTimeSlotDTO
{
    public WeekDays Day { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

public class UpdateTimeSlotsDTO
{
    public List<ScheduledTimeSlotDTO> TimeSlots { get; set; } = new();
}
