using Microsoft.AspNetCore.Mvc;
using horses_for_courses.Core;
using horses_for_courses.Dtos;
using horses_for_courses.Repository;

namespace horses_for_courses.WebApi.Controllers;

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

    [HttpPost]                              // Maakt een nieuwe cursus aan met een naam en een periode.
    public IActionResult CreateCourse([FromBody] CreateCourseDTO dto)
    {
        try
        {
            var period = new PlanningPeriod(dto.StartDate, dto.EndDate);
            var course = Course.Create(dto.CourseName, period);
            _courseRepository.Save(course);

            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]                        // Ondersteunende GET voor CreatedAtAction
    public IActionResult GetCourseById(Guid id)
    {
        var course = _courseRepository.GetById(id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpPost("{id}/skills")]                 // Vervangt de vereiste competenties voor een cursus.
    public IActionResult UpdateCourseCompetences(Guid id, [FromBody] UpdateCourseCompetencesDTO dto)
    {
        var course = _courseRepository.GetById(id);
        if (course is null) return NotFound();

        try
        {
            // Om de lijst te vervangen, moeten we eerst de oude verwijderen.
            // Een 'ClearRequiredCompetencies' methode in de Course class zou hier handig zijn. ZIE COACH!!

            var currentCompetencies = course.RequiredCompetencies.ToList();
            foreach (var competency in currentCompetencies)
            {
                course.RemoveRequiredCompetence(competency);
            }

            // Voeg de nieuwe competenties toe
            foreach (var competency in dto.RequiredCompetences)
            {
                course.AddRequiredCompetence(competency);
            }

            return Ok(course);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/timeslots")]
    public IActionResult UpdateTimeSlots(Guid id, [FromBody] UpdateTimeSlotsDTO dto)
    {
        var course = _courseRepository.GetById(id);
        if (course is null) return NotFound();

        try
        {
            var currentSlots = course.ScheduledTimeSlots.ToList();
            foreach (var slot in currentSlots)
            {
                course.RemoveScheduledTimeSlot(slot);
            }

            foreach (var slotDto in dto.TimeSlots)              // Voeg de nieuwe lesmomenten toe
            {
                var timeSlot = new TimeSlot(slotDto.StartTime, slotDto.EndTime);
                var scheduledSlot = new ScheduledTimeSlot(slotDto.Day, timeSlot);
                course.AddScheduledTimeSlot(scheduledSlot);
            }

            return Ok(course);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/confirm")]                                  // Bevestigt een cursus zodat een coach toegewezen kan worden.
    public IActionResult ConfirmCourse(Guid id)
    {
        var course = _courseRepository.GetById(id);
        if (course is null) return NotFound();

        try
        {
            course.Confirm();
            return Ok(course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/assign-coach")]                             // Wijst een geschikte coach toe aan een bevestigde cursus.
    public IActionResult AssignCoach(Guid id, [FromBody] AssignCoachDTO dto)
    {
        var course = _courseRepository.GetById(id);
        if (course is null) return NotFound($"Cursus met ID {id} niet gevonden.");

        var coach = _coachRepository.GetById(dto.CoachId);
        if (coach is null) return NotFound($"Coach met ID {dto.CoachId} niet gevonden.");

        try
        {
            course.AssignCoach(coach);
            return Ok(course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}