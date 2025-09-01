using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.Dtos;
using HorsesForCourses.Application;
using HorsesForCourses.Infrastructure;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("coaches")]
public class CoachesController : ControllerBase
{
    private readonly ICoachRepository _coachRepository;
    private readonly ICourseRepository _courseRepository;
    public CoachesController(ICoachRepository coachrepository, ICourseRepository courseRepository)
    {
        _coachRepository = coachrepository;
        _courseRepository = courseRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCoach([FromBody] CreateCoachDTO dto)
    {
        var coach = new Coach(FullName.From(dto.Name), EmailAddress.Create(dto.Email));
        await _coachRepository.AddAsync(coach);
        await _coachRepository.SaveChangesAsync();

        return Ok(coach.Id);

        // var coachDto = CoachMapper.ToDTO(coach);             //indien Id enkel beschikbaar na mapping
        // return Ok(coachDto.Id);
    }

    [HttpPost("{id}/skills")]
    public async Task<IActionResult> UpdateCoachSkills([FromBody] UpdateCoachSkillsDTO dto, int id)
    {
        var coach = await _coachRepository.GetByIdAsync(id);
        if (coach is null) return NotFound();

        coach.UpdateSkills(dto.Skills);

        await _coachRepository.SaveChangesAsync();

        // var coachDto = CoachMapper.ToDTO(coach);
        return Ok(dto);
    }

    // [HttpGet]  //no paging
    // public async Task<ActionResult<IEnumerable<CoachDTO>>> GetAll()
    // {
    //     var coaches = await _coachRepository.GetAllAsync();
    //     var allCourses = await _courseRepository.GetAllAsync();
    //     var coachDtos = CoachMapper.ToDTOList(coaches, allCourses);
    //     return Ok(coachDtos);
    // }

    [HttpGet]   //Paging
    public async Task<ActionResult<PagedResult<CoachDTOPaging>>> GetAll([FromQuery] PageRequest request)
    {
        var pagedCoaches = await _coachRepository.GetAllPagedAsync(request);
        return Ok(pagedCoaches);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCoachById(int id)
    {
        var coach = await _coachRepository.GetByIdAsync(id);
        if (coach is null) return NotFound();

        var allCourses = await _courseRepository.GetAllAsync();
        var coachDto = CoachMapper.ToDetailDTO(coach, allCourses);
        return Ok(coachDto);
    }
}
