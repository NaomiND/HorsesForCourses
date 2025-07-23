// public class InMemoryCoachRepository        TODO
// {
//     private readonly Dictionary<Guid, Coach> _coaches = new();

// public void Add(Coach coach)
// {
//     _coaches[coach.Id] = coach;
// }

// public Coach? GetById(Guid id)
// {
//     return _coaches.TryGetValue(id, out var coach) ? coach : null;
// }

// ...
// }