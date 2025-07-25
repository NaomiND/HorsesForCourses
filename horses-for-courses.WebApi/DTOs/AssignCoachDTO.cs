using horses_for_courses.Core;
namespace horses_for_courses.Dtos;

public class AssignCoachDTO
{
    public Guid CourseId { get; set; }
    public Guid CoachId { get; set; }
}