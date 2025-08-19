using HorsesForCourses.Core;
using HorsesForCourses.WebApi;
namespace HorsesForCourses.Infrastructure;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(int id);
    Task<IEnumerable<Course>> GetAllAsync();
    // Task<PagedResult<CourseAssignStatusDTO>> GetAllPagedAsync(PageRequest request); //TODO
    Task AddAsync(Course course);
    Task SaveChangesAsync();
}

//We splitsen Add en Save op. 
//Dit is een veelgebruikt patroon (Unit of Work) waarbij je meerdere wijzigingen kunt groeperen en in één transactie kunt opslaan door SaveChangesAsync aan te roepen.