using HorsesForCourses.Core;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Dtos;

namespace HorsesForCourses.Infrastructure;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(int id);
    Task<IEnumerable<Course>> GetAllAsync();
    Task<PagedResult<CourseAssignStatusDTOPaging>> GetAllPagedAsync(PageRequest request); //TODO
    Task AddAsync(Course course);
    Task SaveChangesAsync();
}
