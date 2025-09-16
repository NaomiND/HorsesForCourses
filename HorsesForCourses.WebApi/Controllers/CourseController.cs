using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.Core;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.common;
using HorsesForCourses.Application;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly CoachAvailability _coachAvailability;

    public CoursesController(ICourseRepository courseRepository, ICoachRepository coachRepository, CoachAvailability coachAvailability)
    {
        _courseRepository = courseRepository;
        _coachRepository = coachRepository;
        _coachAvailability = coachAvailability;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDTO dto)
    {
        var period = new PlanningPeriod(DateOnly.Parse(dto.StartDate), DateOnly.Parse(dto.EndDate));
        var course = Course.Create(dto.Name, period);

        await _courseRepository.AddAsync(course);
        await _courseRepository.SaveChangesAsync();

        return Ok(course.Id);
    }

    // [HttpPost("{id}/skills")]                       // Vervangt de vereiste competenties voor een cursus.
    // public async Task<IActionResult> UpdateCourseSkills([FromBody] UpdateCourseSkillsDTO dto, int id)
    // {
    //     var course = await _courseRepository.GetByIdAsync(id);
    //     if (course is null) return NotFound();

    //     course.UpdateSkills(dto.Skills);

    //     await _courseRepository.SaveChangesAsync();

    //     return Ok(dto);
    // }

    [HttpPost("{id}/timeslots")]        //moet compacter
    public async Task<IActionResult> UpdateTimeSlots([FromBody] UpdateTimeSlotsDTO dto, int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course is null) return NotFound();

        var currentSlots = course.ScheduledTimeSlots.ToList();
        foreach (var slot in currentSlots)
        {
            course.RemoveScheduledTimeSlot(slot);
        }

        foreach (var slotDto in dto.TimeSlots)                      // int starttime en endtime ipv TimeOnly
        {
            var timeSlot = new TimeSlot(slotDto.Start, slotDto.End);                //timeslot.create (voor TimeOnly)
            var scheduledSlot = new ScheduledTimeSlot(slotDto.Day, timeSlot);           //lijsten maken van timeslot en sched slot daarna 
            course.AddScheduledTimeSlot(scheduledSlot);                                 //course.replace met .select van linq
        }

        await _courseRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/confirm")]                                  // Bevestigt een cursus zodat een coach toegewezen kan worden.
    public async Task<IActionResult> ConfirmCourse(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course is null) return NotFound();

        course.Confirm();
        await _courseRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CourseAssignStatusDTOPaging>>> GetAll([FromQuery] PageRequest request)
    {
        var pagedCourses = await _courseRepository.GetAllPagedAsync(request);
        return Ok(pagedCourses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseById(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course is null)
            return NotFound();

        var courseDto = CourseMapper.ToGetByIdResponse(course);
        return Ok(courseDto);
    }

    // [HttpGet("{id}/available-coaches")]
    // public async Task<IActionResult> GetAvailableCoaches(int id)
    // {
    //     var course = await _courseRepository.GetByIdAsync(id);
    //     if (course is null)
    //     {
    //         return NotFound($"Course with Id {id} not found.");
    //     }

    //     if (!course.Skills.Any() || !course.ScheduledTimeSlots.Any())
    //     {
    //         return BadRequest("Course needs to be confirmed first.");
    //     }

    //     var availableCoaches = await _coachRepository.GetAvailableCoachesAsync(course.Skills, course.ScheduledTimeSlots, course.Period);

    //     var coachDtos = availableCoaches.Select(c => new CoachBasicDTO
    //     {
    //         Id = c.Id,
    //         Name = c.Name.ToString()
    //     });

    //     return Ok(coachDtos);
    // }

    // [HttpPost("{id}/assign-coach")]                             // Wijst een geschikte coach toe aan een bevestigde cursus.
    // public async Task<IActionResult> AssignCoach(int id, [FromBody] AssignCoachDTO dto)
    // {
    //     var course = await _courseRepository.GetByIdAsync(id);
    //     if (course is null) return NotFound($"Course with Id {id} not found.");

    //     var coach = await _coachRepository.GetByIdAsync(dto.CoachId);
    //     if (coach is null) return NotFound($"Coach with Id {dto.CoachId} not found.");

    //     var isAvailable = await _coachAvailability.IsCoachAvailableForCourseAsync(coach, course);
    //     if (!isAvailable)
    //     {
    //         throw new DomainException("This coach is unavailable for this course.");
    //     }

    //     course.AssignCoach(coach);
    //     await _courseRepository.SaveChangesAsync();

    //     var courseDto = CourseMapper.ToDTO(course);
    //     return Ok(courseDto);
    // }
}
