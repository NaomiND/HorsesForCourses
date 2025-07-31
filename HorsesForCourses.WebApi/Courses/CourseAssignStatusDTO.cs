using HorsesForCourses.Core;
namespace HorsesForCourses.Dtos;

public class CourseAssignStatusDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public bool HasSchedule { get; set; }
    public bool HasCoach { get; set; }
}