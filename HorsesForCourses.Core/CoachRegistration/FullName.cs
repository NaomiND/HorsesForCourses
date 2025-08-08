namespace HorsesForCourses.Core;

public record FullName
{
    public string FirstName { get; }
    public string LastName { get; }

    public FullName(string firstName, string lastName)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName), "Voornaam verplicht.");
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName), "Achternaam verplicht.");
    }

    public string DisplayName => $"{FirstName} {LastName}";
    public override string ToString() => DisplayName;

    public static FullName From(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Volledige naam verplicht.", nameof(fullName));

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            throw new ArgumentException("Naam moet een voor- en achternaam bevatten.", nameof(fullName));

        return new FullName(parts[0], string.Join(" ", parts.Skip(1)));         //vb "Ine Van Den Broeck" 
    }
}
