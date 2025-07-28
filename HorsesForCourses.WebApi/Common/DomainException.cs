namespace HorsesForCourses.WebApi.Controllers;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}