namespace horses_for_courses.WebApi.Controllers;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}