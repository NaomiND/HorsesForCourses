using HorsesForCourses.Core;

namespace HorsesForCourses.Application.Interfaces;

public interface ISkillRepository
{
    Task<List<Skill>> GetSkills(IEnumerable<string> names);
    Task<List<Skill>> AddSkills(IEnumerable<string> skillNames);
    Task AddAsync(Skill skill);
    Task SaveChangesAsync();

}