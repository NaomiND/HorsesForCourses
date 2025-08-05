using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Infrastructure;

public class EfCoachRepository : ICoachRepository
{
    private readonly AppDbContext _context;

    public EfCoachRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Coach?> GetByIdAsync(int id)
    {
        return await _context.Coaches.FindAsync(id);
    }

    public async Task<IEnumerable<Coach>> GetAllAsync()
    {
        return await _context.Coaches.ToListAsync();
    }

    public async Task AddAsync(Coach coach)
    {
        await _context.Coaches.AddAsync(coach);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
