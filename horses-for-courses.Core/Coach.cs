using System.Linq;

namespace horses_for_courses.Core;

public class Coach
{
    public Guid Id { get; private set; }
    public FullName Name { get; }                               //validation in class FullName
    public EmailAddress Email { get; }                          //validation in class EmailAddress

    private readonly List<string> competenceList = new();       //lijst van competenties (collection)
    public IReadOnlyCollection<string> Competences => competenceList.AsReadOnly();

    public Coach(Guid id, FullName name, EmailAddress email)    //constructor
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public static Coach Create(string name, string email)       //Factory method
    {
        var emailAddress = EmailAddress.From(email);            // string → EmailAddress
        var fullName = FullName.From(name);                     // string → FullName
        return new Coach(Guid.NewGuid(), fullName, emailAddress);
    }

    public void AddCompetence(string competence)
    {
        if (string.IsNullOrWhiteSpace(competence))
            throw new ArgumentException("Competence cannot be empty.");

        if (competenceList.Contains(competence, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Competence already exists.");

        competenceList.Add(competence);
    }

    public void RemoveCompetence(string competence)
    {
        int removedCount = competenceList.RemoveAll(c => string.Equals(c, competence, StringComparison.OrdinalIgnoreCase)); // RemoveAll en StringComparer (hoofdlettergevoelige delete)
        if (removedCount == 0)
            throw new InvalidOperationException($"Competence '{competence}' not found.");
    }

    public void ClearCompetences()
    {
        competenceList.Clear();
    }

    public bool HasAllRequiredCompetences(IEnumerable<string> requiredCompetences)
    {
        if (requiredCompetences == null)
            throw new ArgumentNullException(nameof(requiredCompetences));

        return requiredCompetences.All(rc => competenceList.Contains(rc, StringComparer.OrdinalIgnoreCase));    //Linq
    }
}

// kan niet worden toegewezen aan overlappende opleidingen en moet dus beschikbaar zijn op de ingeplande momenten.