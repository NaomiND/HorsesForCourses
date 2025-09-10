namespace HorsesForCourses.Core;

public class User
{
    public int Id { get; private set; }
    public FullName Name { get; private set; }      //zelfde als coach
    public EmailAddress Email { get; private set; } //zelfde als coach
    public string PasswordHash { get; private set; }
    public Coach? Coach { get; private set; }
    public string Role { get; private set; }//rolebased? mss veranderen voor claim/policy

    private User(FullName name, EmailAddress email, string passwordHash, string role)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

#pragma warning disable CS8618
    private User() { } // Private constructor voor EF Core
#pragma warning restore CS8618

    public static User Create(string name, string email, string plainTextPassword, IPasswordHasher passwordHasher)   //Factory method
    {
        var fullName = FullName.From(name);
        var emailAddress = EmailAddress.Create(email);

        if (string.IsNullOrWhiteSpace(plainTextPassword))
            throw new ArgumentException("Password cannot be empty.", nameof(plainTextPassword));

        if (passwordHasher == null)
            throw new ArgumentNullException(nameof(passwordHasher));

        var passwordHash = passwordHasher.Hash(plainTextPassword);

        // Bepaal de rol. Dit is de admingebruiker.
        string role = (email.ToLower() == "principaluser@admin.com") ? "Admin" : "User";

        return new User(fullName, emailAddress, passwordHash, role);
    }
}
