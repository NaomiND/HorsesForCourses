using System.Net.Mail;

namespace HorsesForCourses.Core;

public record EmailAddress
{
    public string Value { get; }
    private EmailAddress(string value) => Value = value;
    public static bool IsValidEmail(string value) => EmailHelper.IsValidEmail(value);
    public static EmailAddress Create(string value)
    {
        if (!IsValidEmail(value))
            throw new ArgumentException("Invalid e-mailaddress");

        return new EmailAddress(value);
    }

    public override string ToString() => Value;
}

public static class EmailHelper
{
    public static bool IsValidEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) &&
        MailAddress.TryCreate(email, out var addr) &&
        addr.Address == email;
}
