using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Application;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Application.Interfaces;


namespace HorsesForCourses.Infrastructure;

public class EfCoachRepository : ICoachRepository
{
    private readonly AppDbContext _context;
    private readonly ISkillRepository _skillRepository;
    public EfCoachRepository(AppDbContext context, ISkillRepository skillRepository)
    {
        _context = context;
        _skillRepository = skillRepository;
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
            .Include(c => c.CoachSkills)
                .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddSkillToCoach(int coachId, string skillName)
    {
        var coach = await _context.Coaches
            .Include(c => c.CoachSkills)
            .FirstOrDefaultAsync(c => c.Id == coachId);

        if (coach == null) return;

        var skill = (await _skillRepository.AddSkills(new[] { skillName })).FirstOrDefault();
        if (skill == null) return;

        bool alreadyHasSkill = coach.CoachSkills.Any(cs => cs.SkillId == skill.Id);
        if (alreadyHasSkill) return;

        _context.CoachSkills.Add(new CoachSkill { CoachId = coachId, SkillId = skill.Id });
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSkillsAsync(int coachId, IEnumerable<string> newSkillNames)
    {
        var coach = await _context.Coaches
            .Include(c => c.CoachSkills)
            .FirstOrDefaultAsync(c => c.Id == coachId);

        if (coach == null) return;

        var requiredSkills = await _skillRepository.AddSkills(newSkillNames);
        var requiredSkillIds = requiredSkills.Select(s => s.Id).ToList();

        var skillsToRemove = coach.CoachSkills
            .Where(cs => !requiredSkillIds.Contains(cs.SkillId))
            .ToList();
        _context.CoachSkills.RemoveRange(skillsToRemove);

        var currentSkillIds = coach.CoachSkills.Select(cs => cs.SkillId);
        var skillIdsToAdd = requiredSkillIds.Except(currentSkillIds);

        foreach (var skillId in skillIdsToAdd)
        {
            _context.CoachSkills.Add(new CoachSkill { CoachId = coachId, SkillId = skillId });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveSkill(int coachId, string skillName)
    {
        var skillNameToRemove = skillName.Trim().ToLowerInvariant();
        var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Name == skillNameToRemove);

        if (skill == null) return;

        var coachSkillLink = await _context.CoachSkills
            .FirstOrDefaultAsync(cs => cs.CoachId == coachId && cs.SkillId == skill.Id);

        if (coachSkillLink != null)
        {
            _context.CoachSkills.Remove(coachSkillLink);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddAsync(Coach coach)
    {
        await _context.Coaches.AddAsync(coach);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    // public async Task<IEnumerable<Coach>> GetAvailableCoachesAsync(
    // IEnumerable<string> requiredSkills,
    // IEnumerable<ScheduledTimeSlot> slots,
    // PlanningPeriod period)
    // {
    //     var query = _context.Coaches.AsQueryable();

    //     foreach (var skill in requiredSkills)
    //     {
    //         var skillPattern = $"%\"{skill}\"%";
    //         // Gebruik EF.Property om de onderliggende string-kolom aan te spreken
    //         query = query.Where(c => EF.Functions.Like(EF.Property<string>(c, "skills"), skillPattern));
    //     }

    //     query = query.Where(coach =>
    //         !coach.Courses.Any(assignedCourse =>
    //             assignedCourse.Period.StartDate <= period.EndDate &&
    //             assignedCourse.Period.EndDate >= period.StartDate &&

    //             assignedCourse.ScheduledTimeSlots.Any(existingSlot =>
    //                 slots.Any(newSlot =>
    //                     existingSlot.TimeSlot.StartTime < newSlot.TimeSlot.EndTime &&
    //                     existingSlot.TimeSlot.EndTime > newSlot.TimeSlot.StartTime &&
    //                     existingSlot.Day == newSlot.Day
    //                 )
    //             )
    //         )
    //     );
    //     return await query.ToListAsync();
    // }

    // public async Task<IEnumerable<Coach>> GetAvailableCoachesAsync(
    //     IEnumerable<string> requiredSkills,
    //     IEnumerable<ScheduledTimeSlot> slots,
    //     PlanningPeriod period)
    // {
    //     // ---------------------------------------------------------------------------------
    //     // Stap 1: Haal alle coaches op die de vereiste skills hebben.
    //     // We laden ook hun bestaande cursussen en timeslots mee.
    //     // ---------------------------------------------------------------------------------
    //     var query = _context.Coaches
    //         .Include(c => c.Courses)
    //             .ThenInclude(course => course.ScheduledTimeSlots)
    //         .AsQueryable();

    //     if (requiredSkills.Any())
    //     {
    //         foreach (var skill in requiredSkills)
    //         {
    //             coachesQuery = coachesQuery.Where(c => EF.Property<List<string>>(c, "skills").Contains(skill.ToLower()));
    //         }
    //     }

    //     // Voer de database-query uit en haal de kandidaten op.
    //     var candidateCoaches = await query.ToListAsync();

    //     // ---------------------------------------------------------------------------------
    //     // Stap 2: Filter de kandidaten in-memory op beschikbaarheid.
    //     // ---------------------------------------------------------------------------------
    //     var availableCoaches = new List<Coach>();
    //     foreach (var coach in candidateCoaches)
    //     {
    //         // Controleer of deze coach een conflicterende cursus heeft.
    //         bool hasConflict = coach.Courses.Any(assignedCourse =>
    //         {
    //             // Voorwaarde A: De planningperiodes overlappen.
    //             bool periodOverlaps = assignedCourse.Period.StartDate <= period.EndDate &&
    //                                   assignedCourse.Period.EndDate >= period.StartDate;

    //             if (!periodOverlaps)
    //             {
    //                 return false; // Geen periode-overlap, dus geen conflict met deze cursus.
    //             }

    //             // Voorwaarde B: Er is een timeslot-conflict binnen die overlappende periode.
    //             bool timeslotOverlaps = assignedCourse.ScheduledTimeSlots.Any(existingSlot =>
    //                 slots.Any(newSlot =>
    //                     existingSlot.Day == newSlot.Day &&
    //                     existingSlot.TimeSlot.StartTime < newSlot.TimeSlot.EndTime &&
    //                     existingSlot.TimeSlot.EndTime > newSlot.TimeSlot.StartTime
    //                 )
    //             );

    //             return timeslotOverlaps;
    //         });

    //         // Als de coach GEEN conflict heeft, is hij/zij beschikbaar.
    //         if (!hasConflict)
    //         {
    //             availableCoaches.Add(coach);
    //         }
    //     }

    //     return availableCoaches;
    // }

    // public async Task<IEnumerable<Coach>> GetAvailableCoachesAsync(
    //     IEnumerable<string> requiredSkills,
    //     IEnumerable<ScheduledTimeSlot> slots,
    //     PlanningPeriod period)
    // {
    //     // ---------------------------------------------------------------------------------
    //     // STAP 1: Haal ALLE coaches op met hun skills en geplande cursussen.
    //     // ---------------------------------------------------------------------------------
    //     var candidateCoaches = await _context.Coaches
    //         .Include(c => c.Courses)
    //             .ThenInclude(course => course.ScheduledTimeSlots)
    //         .ToListAsync(); // alles laden

    //     // ---------------------------------------------------------------------------------
    //     // STAP 2: Filter de kandidaten op basis van skills
    //     // ---------------------------------------------------------------------------------
    //     if (requiredSkills.Any())
    //     {
    //         candidateCoaches = candidateCoaches
    //             .Where(coach => coach.HasAllRequiredSkills(requiredSkills))
    //             .ToList();
    //     }

    //     // ---------------------------------------------------------------------------------
    //     // STAP 3: Filter op beschikbaarheid
    //     // ---------------------------------------------------------------------------------
    //     var availableCoaches = new List<Coach>();
    //     foreach (var coach in candidateCoaches)
    //     {
    //         bool hasConflict = coach.Courses.Any(assignedCourse =>
    //         {
    //             bool periodOverlaps = assignedCourse.Period.StartDate <= period.EndDate &&
    //                                   assignedCourse.Period.EndDate >= period.StartDate;

    //             if (!periodOverlaps) return false;

    //             bool timeslotOverlaps = assignedCourse.ScheduledTimeSlots.Any(existingSlot =>
    //                 slots.Any(newSlot =>
    //                     existingSlot.Day == newSlot.Day &&
    //                     existingSlot.TimeSlot.StartTime < newSlot.TimeSlot.EndTime &&
    //                     existingSlot.TimeSlot.EndTime > newSlot.TimeSlot.StartTime
    //                 )
    //             );
    //             return timeslotOverlaps;
    //         });

    //         if (!hasConflict)
    //         {
    //             availableCoaches.Add(coach);
    //         }
    //     }

    //     return availableCoaches;
    // }

    public async Task<IEnumerable<Coach>> GetAvailableCoachesAsync(
     IEnumerable<string> requiredSkillNames,
     IEnumerable<ScheduledTimeSlot> slots,
     PlanningPeriod period)
    {
        // Converteer skill-namen naar Id's
        var requiredSkillIds = await _context.Skills
            .Where(s => requiredSkillNames.Contains(s.Name))
            .Select(s => s.Id)
            .ToListAsync();

        if (requiredSkillIds.Count != requiredSkillNames.Count())
        {
            // Eén of meer skills bestaan niet in de database.
            return new List<Coach>();
        }

        var query = _context.Coaches.AsQueryable();

        // Stap 1: Filter op coaches die ALLE vereiste skills bezitten via JOINs.
        foreach (var skillId in requiredSkillIds)
        {
            query = query.Where(c => c.CoachSkills.Any(cs => cs.SkillId == skillId));
        }

        // Stap 2: Filter op beschikbaarheid (deze logica blijft grotendeels hetzelfde)
        query = query.Where(coach =>
            !coach.Courses.Any(assignedCourse =>
                assignedCourse.Period.StartDate <= period.EndDate && assignedCourse.Period.EndDate >= period.StartDate
                &&
                assignedCourse.ScheduledTimeSlots.Any(existingSlot =>
                    slots.Any(newSlot =>
                        existingSlot.Day == newSlot.Day &&
                        existingSlot.TimeSlot.StartTime < newSlot.TimeSlot.EndTime &&
                        existingSlot.TimeSlot.EndTime > newSlot.TimeSlot.StartTime
                    )
                )
            )
        );

        return await query
            .Include(c => c.Courses)
            .ThenInclude(course => course.ScheduledTimeSlots)
            .ToListAsync();
    }

}

