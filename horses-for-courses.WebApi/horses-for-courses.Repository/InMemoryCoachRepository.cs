using horses_for_courses.Core;
namespace horses_for_courses.Repository;

public class InMemoryCoachRepository        //TODO
{
    private readonly Dictionary<Guid, Coach> _coaches = new();

    public Coach? GetById(Guid id)
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

