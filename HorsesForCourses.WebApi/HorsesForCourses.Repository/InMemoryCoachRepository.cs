using HorsesForCourses.Core;
namespace HorsesForCourses.Repository;

public class InMemoryCoachRepository
{
    private readonly Dictionary<int, Coach> _coaches = new();

    public Coach? GetById(int id)
    {
        return _coaches.TryGetValue(id, out var coach) ? coach : null;
    }

    public IEnumerable<Coach> GetAll()
    {
        return _coaches.Values;
    }

    public void Save(Coach coach)
    {
        _coaches[coach.Id] = coach;
    }
}

