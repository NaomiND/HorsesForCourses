using HorsesForCourses.Application.Interfaces;
using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Application;

public class EfSkillRepository : ISkillRepository
{
    private readonly AppDbContext _context;

    public EfSkillRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Skill>> GetSkillsByNamesAsync(IEnumerable<string> names)
    {
        var lowerCaseNames = names.Select(n => n.ToLowerInvariant()).ToList();
        return await _context.Skills
            .Where(s => lowerCaseNames.Contains(s.Name))
            .ToListAsync();
    }

    public async Task<List<Skill>> AddSkills(IEnumerable<string> skillNames)
    {
        var distinctSkillNames = skillNames
            .Select(s => s.Trim().ToLowerInvariant())
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .ToList();

        if (!distinctSkillNames.Any())
        {
            return new List<Skill>();
        }

        var existingSkills = await _context.Skills
            .Where(s => distinctSkillNames.Contains(s.Name))
            .ToListAsync();

        var existingSkillNames = existingSkills.Select(s => s.Name);

        var newSkillNamesToCreate = distinctSkillNames.Except(existingSkillNames);

        if (newSkillNamesToCreate.Any())
        {
            var newSkills = newSkillNamesToCreate.Select(name => new Skill(name));
            await _context.Skills.AddRangeAsync(newSkills);
            await _context.SaveChangesAsync();
        }

        return await _context.Skills
            .Where(s => distinctSkillNames.Contains(s.Name))
            .ToListAsync();
    }

    public async Task AddAsync(Skill skill)
    {
        await _context.Skills.AddAsync(skill);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}