using horses_for_courses.Core;

namespace horses_for_courses.Dtos;

public class CoachDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; }
    public IReadOnlyCollection<string> Competences { get; set; } = [];
}