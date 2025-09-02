namespace HorsesForCourses.Application.dtos;

public class CreateCoachDTO               //Meestal get; set;: standaard en meest flexibele benadering voor DTO's, vanwege serialisatie/deserialisatie.
{
    public string Name { get; set; }
    public string Email { get; set; }
}