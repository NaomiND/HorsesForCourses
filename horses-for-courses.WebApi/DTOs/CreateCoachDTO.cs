namespace horses_for_courses.Dtos;

public class CreateCoachDTO               //Meestal get; set;: standaard en meest flexibele benadering voor DTO's, vanwege serialisatie/deserialisatie.
{
    public string Name { get; set; }
    public string Email { get; set; }
}

//gebruik hier: var dto = coachMapper.ConvertToDTO(coach);
//kan ook gebruikt worden met extensiemethode: public static CoachDTO ConvertToDTO(this coach coach)
//var dto = coach.ConvertToDTO();