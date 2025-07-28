using HorsesForCourses.Core;
namespace HorsesForCourses.Dtos;

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
