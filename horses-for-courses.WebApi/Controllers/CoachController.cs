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

    [HttpPost]
    public IActionResult RegisterCoach([FromBody] CreateCoachDTO dto)
    {
        var coach = _repository.GetById(Guid.NewGuid());
        new Coach(Guid.NewGuid(), FullName.From(dto.Name), EmailAddress.From(dto.Email));
        _repository.Save(coach);

        return Ok();
    }



    // [HttpPost("{id}/skills")]
    // public IActionResult UpdateCoachCompetences(Guid id, [FromBody] UpdateCoachCompetencesDTO dto)
    // {
    //     var coach = _repository.GetById(id);
    //     var dto = new UpdateCoachCompetencesDTO().competenceList;
    //     if (coach is null)
    //     {
    //         return NotFound();
    //     }

    // }

    [HttpGet("/coaches/{id}")]
    public ActionResult<Coach> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        return coach is null ? NotFound() : Ok(coach);
    }
}


/*   [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
 dit is het voorbeeld van weatherforecastcontroller, hier staat ook nog een logger bij, is dat hier ook nodig?*/
