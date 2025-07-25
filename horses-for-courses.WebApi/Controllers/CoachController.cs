using Microsoft.AspNetCore.Mvc;
using horses_for_courses.Core;
using horses_for_courses.Dtos;
using horses_for_courses.Repository;

namespace horses_for_courses.WebApi.Controllers;

[ApiController]
[Route("coaches")]
public class CoachesController : ControllerBase
{
    private readonly InMemoryCoachRepository _repository;

    public CoachesController(InMemoryCoachRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]                                  //Registreert een nieuwe coach met naam en e-mailadres.
    public IActionResult RegisterCoach([FromBody] CreateCoachDTO dto)
    {
        var coach = _repository.GetById(Guid.NewGuid());
        new Coach(Guid.NewGuid(), FullName.From(dto.Name), EmailAddress.From(dto.Email));
        _repository.Save(coach);

        return Ok();
    }
    /*   [HttpPost]
        public IActionResult RegisterCoach([FromBody] CreateCoachDTO dto)
        {
            try
            {
                var coach = Coach.Create(dto.Name, dto.Email);
                _coachRepository.Save(coach); // Gebruik Save of Add uit je repo

                // Stuur een response terug die de locatie van de nieuwe coach aangeeft
                return CreatedAtAction(nameof(GetCoachById), new { id = coach.Id }, coach);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    */

    [HttpPost("{id}/skills")]                   //Vervangt de competenties (skills) van een specifieke coach.
    public IActionResult UpdateCoachCompetences(Guid id, [FromBody] UpdateCoachCompetencesDTO dto)
    {
        var coach = _repository.GetById(id);
        if (coach is null)
        {
            return NotFound();
        }
        try
        {
            coach.ClearCompetences();           // Maak de lijst leeg en vul deze opnieuw met de skills uit de DTO

            foreach (var competence in dto.Competences)
            {
                coach.AddCompetence(competence);
            }
            _repository.Save(coach);

            return Ok(coach);                   // Stuur bijgewerkte coach terug
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Coach> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        return coach is null ? NotFound() : Ok(coach);
    }
}