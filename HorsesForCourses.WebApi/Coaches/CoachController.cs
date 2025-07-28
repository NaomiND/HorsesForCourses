using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.Dtos;
using HorsesForCourses.Repository;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("coaches")]
public class CoachesController : ControllerBase
{
    private readonly InMemoryCoachRepository _coachRepository;

    public CoachesController(InMemoryCoachRepository coachrepository)
    {
        _coachRepository = coachrepository;
    }


    [HttpPost]
    public IActionResult RegisterCoach([FromBody] CreateCoachDTO dto)
    {
        var coach = new Coach(Guid.NewGuid(), FullName.From(dto.Name), EmailAddress.From(dto.Email));
        _coachRepository.Save(coach);

        var coachDto = CoachMapper.ToDTO(coach);
        return Ok(coachDto);
    }

    [HttpPost("{id}/skills")]
    public IActionResult UpdateCoachCompetences([FromBody] UpdateCoachCompetencesDTO dto, Guid id)
    {
        var coach = _coachRepository.GetById(id);
        if (coach is null)
            return NotFound();

        coach.UpdateCompetences(dto.Competences);

        _coachRepository.Save(coach);

        var coachDto = CoachMapper.ToDTO(coach);
        return Ok(coachDto);
    }

    [HttpGet("{id}")]
    public IActionResult GetCoachById(Guid id)
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