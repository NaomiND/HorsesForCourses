namespace HorsesForCourses.Core;

public record FullName
{
    public string FirstName { get; }
    public string LastName { get; }

    public FullName(string firstName, string lastName)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName), "First name required.");
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName), "Last name required.");
    }

    public string DisplayName => $"{FirstName} {LastName}";
    public override string ToString() => DisplayName;

    public static FullName From(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name required.");

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            throw new ArgumentException("Name consist of first name and last name.");

        return new FullName(parts[0], string.Join(" ", parts.Skip(1)));         //vb "Ine Van Den Broeck" 
    }
}
