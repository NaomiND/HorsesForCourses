using HorsesForCourses.Core;
namespace HorsesForCourses.Application.dtos;


public class ScheduledTimeSlotDTO
{
    public WeekDays Day { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
}

public class UpdateTimeSlotsDTO
{
    public List<ScheduledTimeSlotDTO> TimeSlots { get; set; } = new();
}
