using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.Dtos;
using HorsesForCourses.Repository;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("courses")]
public class CoursesController : ControllerBase
{
    private readonly InMemoryCourseRepository _courseRepository;
    private readonly InMemoryCoachRepository _coachRepository;

    public CoursesController(InMemoryCourseRepository courseRepository, InMemoryCoachRepository coachRepository)
    {
        _courseRepository = courseRepository;
        _coachRepository = coachRepository;
    }

    [HttpPost]                                  // Maakt een nieuwe cursus aan met een naam en een periode.
    public IActionResult CreateCourse([FromBody] CreateCourseDTO dto)
    {
        var period = new PlanningPeriod(DateOnly.Parse(dto.StartDate), DateOnly.Parse(dto.EndDate));
        var course = Course.Create(dto.Name, period);

        _courseRepository.Save(course);

        return Ok(course.Id);
        // var courseDto = CourseMapper.ToDTO(course);
        // return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, courseDto);
    }

    [HttpPost("{id}/skills")]                       // Vervangt de vereiste competenties voor een cursus.
    public IActionResult UpdateCourseSkills([FromBody] UpdateCourseSkillsDTO dto, int id)
    {
        var course = _courseRepository.GetById(id);
        if (course is null) return NotFound();

        course.UpdateSkills(dto.Skills);

        _courseRepository.Save(course);

        // var courseDto = CourseMapper.ToDTO(course);
        return Ok(dto);
    }

    [HttpPost("{id}/timeslots")]
    public IActionResult UpdateTimeSlots([FromBody] UpdateTimeSlotsDTO dto, int id)
    {
        var course = _courseRepository.GetById(id);
        if (course is null) return NotFound();

        var currentSlots = course.ScheduledTimeSlots.ToList();
        foreach (var slot in currentSlots)
        {
            course.RemoveScheduledTimeSlot(slot);
        }

        foreach (var slotDto in dto.TimeSlots)
        {
            var timeSlot = new TimeSlot(new TimeOnly(slotDto.Start, 0), new TimeOnly(slotDto.End, 0));
            var scheduledSlot = new ScheduledTimeSlot(slotDto.Day, timeSlot);
            course.AddScheduledTimeSlot(scheduledSlot);
        }
        // var courseDto = CourseMapper.ToDTO(course);
        return NoContent();
    }

    [HttpPost("{id}/confirm")]                                  // Bevestigt een cursus zodat een coach toegewezen kan worden.
    public IActionResult ConfirmCourse(int id)
    {
        var course = _courseRepository.GetById(id);
        if (course is null) return NotFound();

        course.Confirm();

        // var courseDto = CourseMapper.ToDTO(course);
        return NoContent();
    }

    [HttpPost("{id}/assign-coach")]                             // Wijst een geschikte coach toe aan een bevestigde cursus.
    public IActionResult AssignCoach(int id, [FromBody] AssignCoachDTO dto)
    {
        var course = _courseRepository.GetById(id);
        if (course is null) return NotFound($"Cursus met ID {id} niet gevonden.");

        var coach = _coachRepository.GetById(dto.CoachId);
        if (coach is null) return NotFound($"Coach met ID {dto.CoachId} niet gevonden.");

        course.AssignCoach(coach);

        var courseDto = CourseMapper.ToDTO(course);
        return Ok(courseDto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CourseAssignStatusDTO>> GetAll()
    {
        var courses = _courseRepository.GetAll();
        var courseAssignStatusDTO = CourseMapper.ToAssignmentStatusDTOList(courses);
        return Ok(courseAssignStatusDTO);
    }

    [HttpGet("{id}")]
    public IActionResult GetCourseById(int id)
    {
        var course = _courseRepository.GetById(id);
        if (course is null)
            return NotFound();

        var courseDto = CourseMapper.ToGetByIdResponse(course);
        return Ok(courseDto);
    }
}