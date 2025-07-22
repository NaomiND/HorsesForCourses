namespace horses_for_courses.Core;

public record FullName
{
    public string FirstName { get; }
    public string LastName { get; }

    public FullName(string firstName, string lastName)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName), "First name is required.");
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName), "Last name is required.");
    }

    public string DisplayName => $"{FirstName} {LastName}";
    public override string ToString() => DisplayName;                       //is dit dubbel? ToString kan weg ?

    public static FullName From(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            throw new ArgumentException("Name has at least a first and last name.", nameof(fullName));

        return new FullName(parts[0], string.Join(" ", parts.Skip(1)));         //vb "Ine Van Den Broeck" 
    }
}
/* RECORD OF CLASS?
public class FullName
{
    public string FirstName { get; }
    public string LastName { get; }

    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentNullException(nameof(firstName), "First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentNullException(nameof(lastName), "Last name is required.");

        FirstName = firstName;
        LastName = lastName;
    }

    public string DisplayName => $"{FirstName} {LastName}";

    public static FullName From(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            throw new ArgumentException("Name has at least a first and last name.", nameof(fullName));

        var firstName = parts[0];
        var lastName = string.Join(' ', parts.Skip(1));

        return new FullName(firstName: firstName, lastName: lastName);
    }

    public override string ToString() => DisplayName;
}
*/
public record EmailAddress
{
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email mag niet leeg zijn.");
        if (!value.Contains("@"))
            throw new ArgumentException("Ongeldig emailformaat.");
        return new EmailAddress(value);
    }
    public override string ToString() => Value;
}