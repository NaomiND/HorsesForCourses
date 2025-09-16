using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Application;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Application.Interfaces;
using HorsesForCourses.Application.Common;


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

    //onderstaande werkt tot timeslots = veranderen naar een OR via PredicateBuilder
    // public async Task<IEnumerable<Coach>> GetAvailableCoachesAsync(
    //  IEnumerable<string> requiredSkillNames,
    //  IEnumerable<ScheduledTimeSlot> slots,
    //  PlanningPeriod period)
    // {
    //     // Converteer skill-namen naar Id's
    //     var requiredSkillIds = await _context.Skills
    //         .Where(s => requiredSkillNames.Contains(s.Name))
    //         .Select(s => s.Id)
    //         .ToListAsync();

    //     if (requiredSkillIds.Count != requiredSkillNames.Count())
    //     {
    //         // Eén of meer skills bestaan niet in de database.
    //         return new List<Coach>();
    //     }

    //     var query = _context.Coaches.AsQueryable();

    //     // Stap 1: Filter op coaches die ALLE vereiste skills bezitten via JOINs.
    //     foreach (var skillId in requiredSkillIds)
    //     {
    //         query = query.Where(c => c.CoachSkills.Any(cs => cs.SkillId == skillId));
    //     }

    //     // Stap 2: Filter op beschikbaarheid (deze logica blijft grotendeels hetzelfde)
    //     query = query.Where(coach =>
    //         !coach.Courses.Any(assignedCourse =>
    //             assignedCourse.Period.StartDate <= period.EndDate && assignedCourse.Period.EndDate >= period.StartDate
    //             &&
    //             assignedCourse.ScheduledTimeSlots.Any(existingSlot =>
    //                 slots.Any(newSlot =>
    //                     existingSlot.Day == newSlot.Day &&
    //                     existingSlot.TimeSlot.StartTime < newSlot.TimeSlot.EndTime &&
    //                     existingSlot.TimeSlot.EndTime > newSlot.TimeSlot.StartTime
    //                 )
    //             )
    //         )
    //     );

    //     return await query
    //         .Include(c => c.Courses)
    //         .ThenInclude(course => course.ScheduledTimeSlots)
    //         .ToListAsync();
    // }


    public async Task<IEnumerable<Coach>> GetAvailableCoachesAsync(
        IEnumerable<string> requiredSkillNames,
        IEnumerable<ScheduledTimeSlot> slots,
        PlanningPeriod period)
    {
        // Stap 1: Brede DATABASE-query
        // We halen een lijst van kandidaat-coaches op die aan de vertaalbare voorwaarden voldoen (skills).
        // We laden (.Include) hun agenda's mee voor de volgende stap.

        var requiredSkillIds = await _context.Skills
            .Where(s => requiredSkillNames.Contains(s.Name))
            .Select(s => s.Id)
            .ToListAsync();

        if (requiredSkillIds.Count() != requiredSkillNames.Count())
        {
            return new List<Coach>();
        }

        var query = _context.Coaches.AsQueryable();

        foreach (var skillId in requiredSkillIds)
        {
            query = query.Where(c => c.CoachSkills.Any(cs => cs.SkillId == skillId));
        }

        // Materialiseer de kandidaten naar het C#-geheugen
        var candidateCoaches = await query
            .Include(c => c.Courses)
            .ThenInclude(course => course.ScheduledTimeSlots)
            .ToListAsync();

        // Stap 2: Precieze IN-MEMORY filtering
        // Nu we een kleinere lijst in het geheugen hebben, kunnen we complexe C#-logica gebruiken.
        var availableCoaches = new List<Coach>();

        foreach (var coach in candidateCoaches)
        {
            bool hasConflict = coach.Courses.Any(assignedCourse =>
            {
                bool periodOverlaps = assignedCourse.Period.StartDate <= period.EndDate &&
                                      assignedCourse.Period.EndDate >= period.StartDate;

                if (!periodOverlaps) return false;

                // Dit is nu een simpele LINQ-to-Objects .Any() aanroep die werkt met Funcs.
                return assignedCourse.ScheduledTimeSlots.Any(existingSlot =>
                    slots.Any(newSlot => existingSlot.OverlapsWith(newSlot))
                );
            });

            if (!hasConflict)
            {
                availableCoaches.Add(coach);
            }
        }

        return availableCoaches;
    }

}

