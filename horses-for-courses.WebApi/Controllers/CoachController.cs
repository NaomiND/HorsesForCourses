using Microsoft.AspNetCore.Mvc;
using horses_for_courses.Core;

namespace horses_for_courses.WebApi.Controllers;

[ApiController]
[Route("coaches")]
public class CoachController : ControllerBase   //to do
{
    private readonly InMemoryCoachRepository _repository;

    public CoachController(InMemoryCoachRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public ActionResult<Coach> GetById(Guid id)
    {
        var coach = _repository.GetById(id);

        //nog een deel met map .dto (je mag geen domeinobject teruggeven)
        return coach is null ? NotFound() : Ok(coach);
    }
}

