// File: HorsesForCourses.Infrastructure/Repository/EfUserRepository.cs
using HorsesForCourses.Core;
using HorsesForCourses.Application;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Infrastructure
{
    public class EfUserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public EfUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

    //     public async Task<PagedResult<CoachDTOPaging>> GetAllPagedAsync(PageRequest request)
    // {

    //     var query = _context.Coaches                                        // Volgorde: Order -> Project -> Page
    //         .AsNoTracking()
    //         .Include(c => c.Courses)                                        //laadt cursussen
    //         .OrderBy(c => c.Name.LastName).ThenBy(c => c.Name.FirstName)    //Stabiele sortering
    //         .Select(c => new CoachDTOPaging                                       //Projectie naar DTO
    //         {
    //             Id = c.Id,
    //             Name = c.Name.FirstName + " " + c.Name.LastName,
    //             Email = c.Email.Value,
    //             NumberOfCoursesAssignedTo = c.Courses.Count()
    //         });

    //     return await query.ToPagedResultAsync(request);                     //Paging toepassen en resultaat ophalen
    // }
}