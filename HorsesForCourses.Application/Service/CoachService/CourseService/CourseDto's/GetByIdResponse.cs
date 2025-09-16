namespace HorsesForCourses.Application.dtos;

public class GetByIdResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<TimeSlotJasonDTO> TimeSlots { get; set; } = new();
    public List<ScheduledTimeSlotDTO> ScheduledTimeSlots { get; set; } = new();
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public CoachBasicDTO? Coach { get; set; }                           // Genest Coach DTO, nullable
    public string Status { get; set; }
    public bool IsConfirmed { get; set; }
    public int? AssignedCoachId { get; set; }   //toegevoegd
}