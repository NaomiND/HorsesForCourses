namespace HorsesForCourses.Application.dtos;

public class UpdateCourseSkillsDTO
{
    public int Id { get; set; }
    public List<string> Skills { get; set; } = new();
}