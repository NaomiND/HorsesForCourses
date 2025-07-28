using Microsoft.AspNetCore.Mvc;
using horses_for_courses.Core;
using horses_for_courses.Dtos;
using horses_for_courses.Repository;

namespace horses_for_courses.WebApi.Controllers;

[ApiController]
[Route("coaches")]
public class CoachesController : ControllerBase
{
    private readonly InMemoryCoachRepository _coachRepository;

    public CoachesController(InMemoryCoachRepository coachrepository)
    {
        _coachRepository = coachrepository;
    }

    [HttpPost]                                  //Registreert een nieuwe coach met naam en e-mailadres.
    public IActionResult RegisterCoach([FromBody] CreateCoachDTO dto)
    {
        var coach = new Coach(Guid.NewGuid(), FullName.From(dto.Name), EmailAddress.From(dto.Email));
        _coachRepository.Save(coach);

        return Ok(coach);
    }

    [HttpPost("{id}/skills")]                   //Vervangt de competenties (skills) van een specifieke coach.
    public IActionResult UpdateCoachCompetences([FromBody] UpdateCoachCompetencesDTO dto, Guid id)
    {
        var coach = _coachRepository.GetById(id);
        if (coach is null) return NotFound();

        coach.ClearCompetences();               // Maak de lijst leeg en vul deze opnieuw met de skills uit de DTO

        foreach (var competence in dto.Competences)
        {
            coach.AddCompetence(competence); //dit mappen voor terug te geven aan (hele foreach en clear) => replace
        }
        _coachRepository.Save(coach);

        return Ok(coach);                       // Stuur bijgewerkte coach terug
    }

    [HttpGet("{id}")]
    public ActionResult<CoachDTO> GetById(Guid id)
    {
        var coach = _coachRepository.GetById(id);
        if (coach is null)
            return NotFound();

        var coachDto = CoachMapper.ToDTO(coach);
        return Ok(coachDto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CoachDTO>> GetAll()
    {
        var coaches = _coachRepository.GetAll();
        var coachDtos = CoachMapper.ToDTOList(coaches);
        return Ok(coachDtos);
    }
}