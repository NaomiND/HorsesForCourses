namespace HorsesForCourses.Application.dtos;

public class UpdateCoachSkillsDTO
{
    public int Id { get; set; }
    public List<string> Skills { get; set; } = new();
}

