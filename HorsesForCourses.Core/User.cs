namespace HorsesForCourses.Core;

public class User
{
    public int Id { get; private set; }
    public FullName Name { get; private set; }      //zelfde als coach
    public EmailAddress Email { get; private set; } //zelfde als coach
    public string PasswordHash { get; private set; }

    private User(FullName name, EmailAddress email, string passwordHash)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }
    private User() { } // Private constructor voor EF Core
    // Factory method die de validatie van de Value Objects gebruikt
    public static User Create(string name, string email, string passwordHash)
    {
        var fullName = FullName.From(name);
        var emailAddress = EmailAddress.Create(email);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        return new User(fullName, emailAddress, passwordHash);
    }
}