using Microsoft.AspNetCore.Mvc;
using horses_for_courses.Core;
using horses_for_courses.Dtos;

namespace horses_for_courses.WebApi.Controllers;

[ApiController]
[Route("[coaches]")]
public class CoachController : ControllerBase
{
    private readonly InMemoryCoachRepository _repository;

    public CoachController(InMemoryCoachRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("/registration")]
    public IActionResult Register([FromBody] CreateCoachDTO dto)
    {
        new Coach(Guid.NewGuid(), FullName.From(dto.Name), EmailAddress.From(dto.Email));

        return Ok();
    }

    [HttpGet("/coaches/{id}")]
    public ActionResult<Coach> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        // var dto = CoachMapper.ConvertToDTO(coach);
        //nog een deel met map .dto (je mag geen domeinobject teruggeven)
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
