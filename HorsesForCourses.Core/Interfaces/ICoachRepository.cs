namespace HorsesForCourses.Core;

public interface ICoachRepository
{
    Task<Coach?> GetByIdAsync(int id);
    Task<IEnumerable<Coach>> GetAllAsync();
    Task AddAsync(Coach coach);
    Task SaveChangesAsync();
}

//We splitsen Add en Save op. 
//Dit is een veelgebruikt patroon (Unit of Work) waarbij je meerdere wijzigingen kunt groeperen en in één transactie kunt opslaan door SaveChangesAsync aan te roepen.