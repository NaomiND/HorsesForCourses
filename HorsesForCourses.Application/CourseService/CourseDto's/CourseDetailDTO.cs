namespace HorsesForCourses.Dtos;

public class CourseDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public List<ScheduledTimeSlotDTO> ScheduledTimeSlots { get; set; } = new();
    public bool IsConfirmed { get; set; }    //public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public int? AssignedCoachId { get; set; }
}