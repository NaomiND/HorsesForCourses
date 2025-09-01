namespace HorsesForCourses.Application.common;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}