using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Application;
using HorsesForCourses.Infrastructure.Extensions;
using HorsesForCourses.Dtos;

namespace HorsesForCourses.Infrastructure;

public class EfCourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public EfCourseRepository(AppDbContext context)
    {
        _context = context;
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

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}