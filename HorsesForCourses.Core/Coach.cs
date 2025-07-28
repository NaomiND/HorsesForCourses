namespace HorsesForCourses.Core;

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
            throw new ArgumentException("Competentie kan niet leeg zijn.");

        if (competenceList.Contains(competence.ToLower()))
            throw new InvalidOperationException("Competentie werd reeds toegevoegd.");

        competenceList.Add(competence.ToLower());
    }

    public void RemoveCompetence(string competence)
    {
        int removedCount = competenceList.RemoveAll(c => string.Equals(c, competence.ToLower())); // RemoveAll en StringComparer (hoofdlettergevoelige delete)
        if (removedCount == 0)
            throw new InvalidOperationException($"Competentie '{competence}' niet gevonden.");
    }

    public void ClearCompetences()
    {
        competenceList.Clear();
    }

    public void UpdateCompetences(IEnumerable<string> newCompetences)
    {
        ClearCompetences();

        foreach (var competence in newCompetences)
        {
            AddCompetence(competence);
        }
    }

    public bool HasAllRequiredCompetences(IEnumerable<string> requiredCompetences)
    {
        List<string> lowerCase = competenceList.Select(x => x.ToLower()).ToList();
        if (requiredCompetences == null)
            throw new ArgumentNullException(nameof(requiredCompetences));

        return requiredCompetences.All(rc => lowerCase.Contains(rc.ToLower()));    //Linq
    }
}
