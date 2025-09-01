using HorsesForCourses.Core;
namespace HorsesForCourses.Dtos;

public class CourseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool HasSchedule { get; set; }   //true
    public bool HasCoach { get; set; }      //false
}