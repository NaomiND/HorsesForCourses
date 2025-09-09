namespace HorsesForCourses.Core;

public class User
{
    public int Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    private User() { } //EFCore

    // public static From(string name, string email, string pass, string confirmPass)

    public User(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }
}