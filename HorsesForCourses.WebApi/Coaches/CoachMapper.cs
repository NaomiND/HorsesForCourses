using HorsesForCourses.Core;

namespace HorsesForCourses.Dtos;

public static class CoachMapper
{
    public static CoachDTO ToDTO(Coach coach)
    {
        return new CoachDTO
        {
            Id = coach.Id,
            Name = coach.Name.ToString(),
            Email = coach.Email.Value,
            Competences = coach.Competences.ToList()
        };
    }

    public static IEnumerable<CoachDTO> ToDTOList(IEnumerable<Coach> coaches)
    {
        return coaches.Select(ToDTO);
    }
}