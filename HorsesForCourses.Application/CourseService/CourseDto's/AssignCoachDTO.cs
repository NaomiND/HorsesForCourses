using HorsesForCourses.MVC.ViewModels;

namespace HorsesForCourses.Application.dtos;

public class AssignCoachDTO
{
    public int CoachId { get; set; }
    public int CourseId { get; set; }
    public List<ListCoaches> AvailableCoaches { get; set; } = new();
}
