using horses_for_courses.Core;
namespace horses_for_courses.Dtos;

public class UpdateCoachCompetencesDTO
{
    public List<string> Competences { get; set; } = new();
}
