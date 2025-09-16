namespace HorsesForCourses.Application.dtos;

public class CourseAssignStatusDTOPaging
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public bool HasSchedule { get; set; }
    public bool HasCoach { get; set; }
    public string Status { get; set; }
}