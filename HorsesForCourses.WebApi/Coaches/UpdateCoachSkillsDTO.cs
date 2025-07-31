namespace HorsesForCourses.Dtos;

public class UpdateCoachSkillsDTO
{
    public int Id { get; set; }
    public List<string> Skills { get; set; } = new();
}

