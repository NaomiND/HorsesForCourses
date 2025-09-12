using HorsesForCourses.Core;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.dtos;

namespace HorsesForCourses.Application;

public interface ICoachRepository
{
    Task<Coach?> GetByIdAsync(int id);
    Task<IEnumerable<Coach>> GetAllAsync();
    Task<PagedResult<CoachDTOPaging>> GetAllPagedAsync(PageRequest request); //TODO
    Task AddAsync(Coach coach);
    Task SaveChangesAsync();
    Task AddSkillToCoach(int coachId, string skillName);
    Task UpdateSkillsAsync(int coachId, IEnumerable<string> newSkillNames);
    Task RemoveSkill(int coachId, string skillName);
    Task<IEnumerable<Coach>> GetAvailableCoachesAsync(
        IEnumerable<string> requiredSkills,
        IEnumerable<ScheduledTimeSlot> slots,
        PlanningPeriod period);
}

//We splitsen Add en Save op. 
//Dit is een veelgebruikt patroon (Unit of Work) waarbij je meerdere wijzigingen kunt groeperen en in één transactie kunt opslaan door SaveChangesAsync aan te roepen.