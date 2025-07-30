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
        var coach = new Coach(FullName.From(dto.Name), EmailAddress.From(dto.Email));
        _coachRepository.Save(coach);

        return Ok(coach.Id);
        // var coachDto = CoachMapper.ToDTO(coach);             //indien Id enkel beschikbaar na mapping
        // return Ok(coachDto.Id);
    }

    [HttpPost("{id}/skills")]
    public IActionResult UpdateCoachSkills([FromBody] UpdateCoachSkillsDTO dto, int id)
    {
        var coach = _coachRepository.GetById(id);
        if (coach is null)
            return NotFound();

        coach.UpdateSkills(dto.Skills);

        _coachRepository.Save(coach);

        var coachDto = CoachMapper.ToDTO(coach);        //TODO
        return Ok(coachDto);
    }

    [HttpGet("{id}")]
    public IActionResult GetCoachById(int id)
    {
        var coach = _coachRepository.GetById(id);
        if (coach is null)
            return NotFound();

        var coachDto = CoachMapper.ToDTO(coach);        //TODO
        return Ok(coachDto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CoachDTO>> GetAll()
    {
        var coaches = _coachRepository.GetAll();
        var allCourses = _courseRepository.GetAll();        //waaaaarommmmm??
        var coachDtos = CoachMapper.ToDTOList(coaches, allCourses);
        return Ok(coachDtos);
    }
}