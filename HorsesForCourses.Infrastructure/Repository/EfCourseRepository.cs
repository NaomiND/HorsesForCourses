using HorsesForCourses.Core;
using HorsesForCourses.infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Infrastructure;

public class EfCourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public EfCourseRepository(AppDbContext context)
    {
        _context = context;
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