using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.Application;

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

        await _courseRepository.GetCoursesByCoachIdAsync(id);

        var coachDto = CoachMapper.ToDetailDTO(coach);
        return Ok(coachDto);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCoach([FromBody] CreateCoachDTO dto)
    {
        var coach = new Coach(FullName.From(dto.Name), EmailAddress.Create(dto.Email));
        await _coachRepository.AddAsync(coach);
        await _coachRepository.SaveChangesAsync();

        return Ok(coach.Id);
    }

    [HttpPost("{id}/skills")]
    public async Task<IActionResult> UpdateCoachSkills(int id, [FromBody] UpdateCoachSkillsDTO dto)
    {
        if (id != dto.Id)
            return BadRequest("ID in URL and body do not match.");

        var coach = await _coachRepository.GetByIdAsync(id);
        if (coach is null)
            return NotFound();


        await _coachRepository.UpdateSkillsAsync(id, dto.Skills);
        return Ok(dto);
        //return NoContent();
    }

    [HttpGet("{id}/available-coaches")]
    public async Task<IActionResult> GetAvailableCoaches(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course is null)
        {
            return NotFound($"Course with Id {id} not found.");
        }

        if (course.Status != CourseStatus.Confirmed)
            return BadRequest("Coaches can only be assigned to a confirmed course.");

        var requiredSkillNames = course.CourseSkills.Select(cs => cs.Skill.Name);
        var availableCoaches = await _coachRepository.GetAvailableCoachesAsync(requiredSkillNames, course.ScheduledTimeSlots, course.Period);

        var coachDtos = availableCoaches.Select(c => new CoachBasicDTO
        {
            Id = c.Id,
            Name = c.Name.ToString()
        });

        return Ok(coachDtos);
    }
}

