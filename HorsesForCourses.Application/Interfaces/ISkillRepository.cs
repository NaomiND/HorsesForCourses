using HorsesForCourses.Core;

namespace HorsesForCourses.Application.Interfaces;

public interface ISkillRepository
{
    Task<List<Skill>> GetSkillsByNamesAsync(IEnumerable<string> names);
    Task AddAsync(Skill skill);
    Task SaveChangesAsync();
    Task<List<Skill>> AddSkills(IEnumerable<string> skillNames);
}