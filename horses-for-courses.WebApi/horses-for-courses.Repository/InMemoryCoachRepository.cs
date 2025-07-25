using horses_for_courses.Core;
namespace horses_for_courses.Repository;

public class InMemoryCoachRepository        //TODO
{
    private readonly Dictionary<Guid, Coach> _coaches = new();

    public Coach? GetById(Guid id)
    {
        return _coaches.TryGetValue(id, out var coach) ? coach : null;
    }

    public void Add(Coach coach)
    {
        _coaches[coach.Id] = coach;
    }
    public void Save(Coach coach)
    {
        if (_coaches.ContainsKey(coach.Id))
        {
            _coaches[coach.Id] = coach;
        }
        else
        {
            _coaches.Add(coach.Id, coach);
        }
    }
}

