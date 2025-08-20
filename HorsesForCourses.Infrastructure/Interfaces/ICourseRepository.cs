using HorsesForCourses.Core;
using HorsesForCourses.Application;

namespace HorsesForCourses.Infrastructure;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(int id);
    Task<IEnumerable<Course>> GetAllAsync();
    Task<PagedResult<CourseAssignStatusDTOPaging>> GetAllPagedAsync(PageRequest request); //TODO
    Task AddAsync(Course course);
    Task SaveChangesAsync();
}

//We splitsen Add en Save op. 
//Dit is een veelgebruikt patroon (Unit of Work) waarbij je meerdere wijzigingen kunt groeperen en in één transactie kunt opslaan door SaveChangesAsync aan te roepen.

public class CourseAssignStatusDTOPaging
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public bool HasSchedule { get; set; }
    public bool HasCoach { get; set; }
}