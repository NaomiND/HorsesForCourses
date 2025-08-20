using HorsesForCourses.Core;
using HorsesForCourses.Application;

namespace HorsesForCourses.Infrastructure;

public interface ICoachRepository
{
    Task<Coach?> GetByIdAsync(int id);
    Task<IEnumerable<Coach>> GetAllAsync();
    Task<PagedResult<CoachDTOPaging>> GetAllPagedAsync(PageRequest request); //TODO
    Task AddAsync(Coach coach);
    Task SaveChangesAsync();
}

public class CoachDTOPaging
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; }
    public int NumberOfCoursesAssignedTo { get; set; }              //nodig om aantal courses/coach te tonen

}

//We splitsen Add en Save op. 
//Dit is een veelgebruikt patroon (Unit of Work) waarbij je meerdere wijzigingen kunt groeperen en in één transactie kunt opslaan door SaveChangesAsync aan te roepen.