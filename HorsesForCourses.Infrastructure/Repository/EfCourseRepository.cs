using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Application;
using HorsesForCourses.Application.dtos;

using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.Interfaces;

namespace HorsesForCourses.Infrastructure;

public class EfCourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;
    private readonly ISkillRepository _skillRepository;
    public EfCourseRepository(AppDbContext context, ISkillRepository skillRepository)
    {
        _context = context;
        _skillRepository = skillRepository;
    }

    public async Task<PagedResult<CourseAssignStatusDTOPaging>> GetAllPagedAsync(PageRequest request)
    {
        var query = _context.Courses
            .AsNoTracking()
            .OrderBy(c => c.Name)                           //Sorteren
            .Select(c => new CourseAssignStatusDTOPaging          //Projecteren
            {
                Id = c.Id,
                Name = c.Name,
                StartDate = c.Period.StartDate.ToString(),
                EndDate = c.Period.EndDate.ToString(),
                HasSchedule = c.ScheduledTimeSlots.Any(),
                HasCoach = c.AssignedCoach != null
            });

        return await query.ToPagedResultAsync(request);     //Paging
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        // Gebruik Include om gerelateerde data (de coach) mee te laden.
        return await _context.Courses
            .Include(c => c.AssignedCoach)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses
            .Include(c => c.AssignedCoach)
            .ToListAsync();
    }

    public async Task<Course?> GetByIdWithSkillsAsync(int id)
    {
        return await _context.Courses
            .Include(c => c.CourseSkills)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Course>> GetCoursesByCoachIdAsync(int coachId)
    {
        return await _context.Courses
            .Where(c => c.AssignedCoach != null && c.AssignedCoach.Id == coachId)
            .ToListAsync();
    }

    public async Task AddSkillToCourse(int courseId, string skillName)
    {
        var course = await _context.Courses
            .Include(c => c.CourseSkills)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null) return;

        var skill = (await _skillRepository.AddSkills(new[] { skillName })).FirstOrDefault();
        if (skill == null) return;

        bool alreadyHasSkill = course.CourseSkills.Any(cs => cs.SkillId == skill.Id);
        if (alreadyHasSkill)
            return;

        _context.CourseSkills.Add(new CourseSkill { CourseId = courseId, SkillId = skill.Id });
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSkillsAsync(int courseId, IEnumerable<string> newSkillNames)
    {
        var course = await _context.Courses
            .Include(c => c.CourseSkills)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null) return;

        var requiredSkills = await _skillRepository.AddSkills(newSkillNames);
        var requiredSkillIds = requiredSkills.Select(s => s.Id).ToList();

        var skillsToRemove = course.CourseSkills
            .Where(cs => !requiredSkillIds.Contains(cs.SkillId))
            .ToList();
        _context.CourseSkills.RemoveRange(skillsToRemove);

        var currentSkillIds = course.CourseSkills.Select(cs => cs.SkillId);
        var skillIdsToAdd = requiredSkillIds.Except(currentSkillIds);

        foreach (var skillId in skillIdsToAdd)
        {
            _context.CourseSkills.Add(new CourseSkill { CourseId = courseId, SkillId = skillId });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveSkill(int courseId, string skillName)
    {
        var skillNameToRemove = skillName.Trim().ToLowerInvariant();
        var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Name == skillNameToRemove);

        if (skill == null) return;

        var courseSkillLink = await _context.CourseSkills
            .FirstOrDefaultAsync(cs => cs.CourseId == courseId && cs.SkillId == skill.Id);

        if (courseSkillLink != null)
        {
            _context.CourseSkills.Remove(courseSkillLink);
            await _context.SaveChangesAsync();
        }
    }
}