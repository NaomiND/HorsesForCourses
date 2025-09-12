using HorsesForCourses.Core;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.dtos;

namespace HorsesForCourses.Infrastructure;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(int id);
    Task<IEnumerable<Course>> GetAllAsync();
    Task<PagedResult<CourseAssignStatusDTOPaging>> GetAllPagedAsync(PageRequest request); //TODO
    Task AddAsync(Course course);
    Task SaveChangesAsync();
    Task<IEnumerable<Course>> GetCoursesByCoachIdAsync(int coachId);
    Task<Course?> GetByIdWithSkillsAsync(int id);
    Task AddSkillToCourse(int courseId, string skillName);
    Task UpdateSkillsAsync(int courseId, IEnumerable<string> newSkillNames);
    Task RemoveSkill(int courseId, string skillName);
}
