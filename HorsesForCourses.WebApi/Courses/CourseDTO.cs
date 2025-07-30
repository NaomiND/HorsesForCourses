using HorsesForCourses.Core;
namespace HorsesForCourses.Dtos;

public class CourseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PlanningPeriod Period { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> RequiredCompetences { get; set; } = new();
    public List<ScheduledTimeSlotDTO> ScheduledTimeSlots { get; set; } = new();
    public bool IsConfirmed { get; set; }    //public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public int? AssignedCoachId { get; set; }
}
