namespace HorsesForCourses.Core;

public class Skill
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    private Skill() { } // EF Core
    private readonly List<string> skills = new();//dit moet er dus uit maar zo werkt de code nog
    public Skill(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill cannot be empty.", nameof(name));
        Name = name.ToLowerInvariant();
    }

    public void AddSkill(string skill)  //=course & coach
    {
        if (string.IsNullOrWhiteSpace(skill))
            throw new ArgumentException("Skill can't be empty.");

        if (skills.Contains(skill.ToLower()))
            throw new InvalidOperationException("Skill already exists.");

        skills.Add(skill.ToLower());
    }

    public void RemoveSkill(string skill)   //=course & coach
    {
        int removedCount = skills.RemoveAll(c => string.Equals(c, skill.ToLower())); // RemoveAll en StringComparer (hoofdlettergevoelige delete)
        if (removedCount == 0)
            throw new InvalidOperationException($"Skill '{skill}' not found.");
    }

    public void ClearSkills()   //=course & coach
    {
        skills.Clear();
    }

    public void UpdateSkills(IEnumerable<string> newSkills) //=course & coach
    {
        ClearSkills();

        foreach (var skill in newSkills)
        {
            AddSkill(skill);
        }
    }


}
