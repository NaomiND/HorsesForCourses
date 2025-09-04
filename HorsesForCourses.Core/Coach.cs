namespace HorsesForCourses.Core;

public class Coach
{
    public int Id { get; private set; }                         //Id handmatig instellen
    public FullName Name { get; }                               //validation in class FullName
    public EmailAddress Email { get; }                          //validation in class EmailAddress
    private List<string> skills = new();                        //lijst van skills (collection), geen readonly hier(ef kan die niet vullen)
    public IReadOnlyCollection<string> Skills => skills.AsReadOnly();

    public Coach(FullName name, EmailAddress email)             //constructor (bij entity framework wordt ID automatisch gegenereerd door database dus hier te verwijderen)
    {
        Name = name;
        Email = email;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    private Coach() { }                                         // Private constructor voor EF Core
#pragma warning restore CS8618

    public static Coach Create(string name, string email)       //Factory method
    {
        var emailAddress = EmailAddress.Create(email);            // string → EmailAddress
        var fullName = FullName.From(name);                     // string → FullName
        return new Coach(fullName, emailAddress);
    }

    public void AddSkill(string skill)
    {
        if (string.IsNullOrWhiteSpace(skill))
            throw new ArgumentException("Competentie kan niet leeg zijn.");

        if (skills.Contains(skill.ToLower()))
            throw new InvalidOperationException("Competentie werd reeds toegevoegd.");

        skills.Add(skill.ToLower());
    }

    public void RemoveSkill(string skill)
    {
        int removedCount = skills.RemoveAll(c => string.Equals(c, skill.ToLower())); // RemoveAll en StringComparer (hoofdlettergevoelige delete)
        if (removedCount == 0)
            throw new InvalidOperationException($"Competentie '{skill}' niet gevonden.");
    }

    public void ClearSkills()
    {
        skills.Clear();
    }

    public void UpdateSkills(IEnumerable<string> newSkills)
    {
        ClearSkills();

        foreach (var skill in newSkills)
        {
            AddSkill(skill);
        }
    }

    public bool HasAllRequiredSkills(IEnumerable<string> requiredSkills)
    {
        List<string> lowerCase = skills.Select(x => x.ToLower()).ToList();
        if (requiredSkills == null)
            throw new ArgumentNullException(nameof(requiredSkills));

        return requiredSkills.All(rc => lowerCase.Contains(rc.ToLower()));    //Linq
    }
}
