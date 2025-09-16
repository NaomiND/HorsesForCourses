namespace HorsesForCourses.Application.dtos;

public class CoachDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; }
    public List<string> Skills { get; set; } = [];
    public List<CourseBasicInfo> Courses { get; set; } = [];
}