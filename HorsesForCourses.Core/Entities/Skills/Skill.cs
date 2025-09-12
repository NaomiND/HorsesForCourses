namespace HorsesForCourses.Core;

public class Skill
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    private Skill() { } // EF Core
    public Skill(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill cannot be empty.", nameof(name));
        Name = name.ToLowerInvariant();
    }
}
