using horses_for_courses.Core;
namespace horses_for_courses.Dtos;

public class AssignCoachDto
{
    public Guid CourseId { get; set; }
    public Guid CoachId { get; set; }
}