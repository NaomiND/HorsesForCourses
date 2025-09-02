using System.ComponentModel.DataAnnotations;

namespace HorsesForCourses.Application.dtos;

public class CreateCoachDTO               //Meestal get; set;: standaard en meest flexibele benadering voor DTO's, vanwege serialisatie/deserialisatie.
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
}