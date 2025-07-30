namespace HorsesForCourses.Dtos;

public class CoachDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; }
    public IReadOnlyCollection<string> Skills { get; set; } = [];
}