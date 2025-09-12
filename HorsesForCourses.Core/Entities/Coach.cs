namespace HorsesForCourses.Core;

public class Coach
{
    public int Id { get; private set; }                         //Id handmatig instellen
    public FullName Name { get; }                               //validation in class FullName
    public EmailAddress Email { get; }                          //validation in class EmailAddress
    public int? UserId { get; private set; }                    //relatie tss user en coach
    public User? User { get; private set; }                     //relatie tss user en coach
    // private List<string> skills = new();                        //lijst van skills (collection), geen readonly hier(ef kan die niet vullen)
    // public IReadOnlyCollection<string> Skills => skills.AsReadOnly();
    private readonly List<CoachSkill> coachSkills = new();
    public IReadOnlyCollection<CoachSkill> CoachSkills => coachSkills.AsReadOnly();
    private readonly List<Course> courses = new();              //lijst van courses
    public IReadOnlyCollection<Course> Courses => courses.AsReadOnly();
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

    public void AssignUser(User user)
    {
        if (UserId.HasValue)
        {
            throw new InvalidOperationException("This coach is already assigned to a user.");
        }
        User = user;    //volledige object gekoppeld voor UOW bij opslaan
    }

    public bool HasAllRequiredSkills(IEnumerable<string> requiredSkillNames)
    {
        if (requiredSkillNames == null || !requiredSkillNames.Any())
            return true;

        var coachSkillNames = this.CoachSkills
            .Select(cs => cs.Skill.Name)
            .ToHashSet(); // ToHashSet is efficiënt voor lookups.

        return requiredSkillNames.All(reqSkill => coachSkillNames.Contains(reqSkill.Trim().ToLowerInvariant()));
    }
}
