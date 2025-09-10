using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Application;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.dtos;


namespace HorsesForCourses.Infrastructure;

public class EfCoachRepository : ICoachRepository
{
    private readonly AppDbContext _context;

    public EfCoachRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CoachDTOPaging>> GetAllPagedAsync(PageRequest request)
    {

        var query = _context.Coaches                                        // Volgorde: Order -> Project -> Page
            .AsNoTracking()
            .Include(c => c.Courses)                                        //laadt cursussen
            .OrderBy(c => c.Name.LastName).ThenBy(c => c.Name.FirstName)    //Stabiele sortering
            .Select(c => new CoachDTOPaging                                       //Projectie naar DTO
            {
                Id = c.Id,
                Name = c.Name.FirstName + " " + c.Name.LastName,
                Email = c.Email.Value,
                NumberOfCoursesAssignedTo = c.Courses.Count()
            });

        return await query.ToPagedResultAsync(request);                     //Paging toepassen en resultaat ophalen
    }

    public async Task<IEnumerable<Coach>> GetAllAsync()
    {
        return await _context.Coaches
            .Include(c => c.Courses)
            .ToListAsync();
    }

    public async Task<Coach?> GetByIdAsync(int id)
    {
        return await _context.Coaches
            .Include(c => c.Courses)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task AddAsync(Coach coach)
    {
        await _context.Coaches.AddAsync(coach);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Coach>> GetAvailableCoachesAsync(
    IEnumerable<string> requiredSkills,
    IEnumerable<ScheduledTimeSlot> slots,
    PlanningPeriod period)
    {
        var query = _context.Coaches.AsQueryable();

        foreach (var skill in requiredSkills)
        {
            var skillPattern = $"%\"{skill}\"%";
            // Gebruik EF.Property om de onderliggende string-kolom aan te spreken
            query = query.Where(c => EF.Functions.Like(EF.Property<string>(c, "skills"), skillPattern));
        }

        query = query.Where(coach =>
            !coach.Courses.Any(assignedCourse =>
                assignedCourse.Period.StartDate <= period.EndDate &&
                assignedCourse.Period.EndDate >= period.StartDate &&

                assignedCourse.ScheduledTimeSlots.Any(existingSlot =>
                    slots.Any(newSlot =>
                        existingSlot.TimeSlot.StartTime < newSlot.TimeSlot.EndTime &&
                        existingSlot.TimeSlot.EndTime > newSlot.TimeSlot.StartTime &&
                        existingSlot.Day == newSlot.Day
                    )
                )
            )
        );
        return await query.ToListAsync();
    }
}

