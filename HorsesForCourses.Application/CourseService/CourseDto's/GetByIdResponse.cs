namespace HorsesForCourses.Application.dtos;


public class GetByIdResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<TimeSlotJasonDTO> TimeSlots { get; set; } = new();
    public CoachBasicDTO? Coach { get; set; }                           // Genest Coach DTO, nullable
}

