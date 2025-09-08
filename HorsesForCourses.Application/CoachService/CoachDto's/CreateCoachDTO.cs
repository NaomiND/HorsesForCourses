using System.ComponentModel.DataAnnotations;

namespace HorsesForCourses.Application.dtos;

public class CreateCoachDTO
{
    public string Name { get; set; }
    // [EmailAddress]
    public string Email { get; set; }
}

/* ASP.NET Core kijkt naar het type van het Email-veld in CreateCoachDTO. 
Als dat een string is, en er zit [EmailAddress]-attributen of een soortgelijke data-annotatie op, of er is default model validatie actief, 
dan wordt e-mail op modelniveau gevalideerd vóórdat je controller of je EmailAddress.Create() wordt aangeroepen.
Dit kan je gebruiken als je zelf geen validatie instelt in de domeinlaag. 
Geen try catch nodig in je controller*/