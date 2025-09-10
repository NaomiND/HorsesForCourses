using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HorsesForCourses.MVC.CourseController
{
    [Controller]
    [Route("courses")]
    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICoachRepository _coachRepository;


        public CoursesController(ICourseRepository courseRepository, ICoachRepository coachrepository)
        {
            _courseRepository = courseRepository;
            _coachRepository = coachrepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var request = new PageRequest(PageNumber: page, PageSize: pageSize);
            var pagedCourses = await _courseRepository.GetAllPagedAsync(request);
            return View(pagedCourses);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            var CourseDetailDTO = CourseMapper.ToGetByIdResponse(course);
            return View(CourseDetailDTO);
        }

        [Authorize(Policy = "CourseManagement")]
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = "CourseManagement")]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,StartDate,EndDate")] CreateCourseDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var startDate = DateOnly.Parse(dto.StartDate);
                var endDate = DateOnly.Parse(dto.EndDate);
                var period = new PlanningPeriod(startDate, endDate);

                var course = Course.Create(dto.Name, period);

                await _courseRepository.AddAsync(course);
                await _courseRepository.SaveChangesAsync();

                TempData["SuccessMessage"] = "Course registered.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
            catch (FormatException)
            {
                ModelState.AddModelError(string.Empty, "Invalid format.");
                return View(dto);
            }
        }

        [Authorize(Policy = "CourseManagement")]
        [HttpGet("editskills/{id}")]
        public async Task<IActionResult> EditSkills(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            var dto = new UpdateCourseSkillsDTO
            {
                Id = course.Id,
                Skills = course.Skills.ToList()
            };
            ViewBag.CourseName = course.Name;
            return View(dto);
        }

        [Authorize(Policy = "CourseManagement")]
        [HttpPost("editskills/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSkills(int id, [Bind("Id,Skills")] UpdateCourseSkillsDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var courseToUpdate = await _courseRepository.GetByIdAsync(id);
            if (courseToUpdate == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.CourseName = courseToUpdate.Name;
                return View(dto);
            }

            try
            {
                var skillsList = Request.Form["Skills"].ToString()
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                courseToUpdate.UpdateSkills(skillsList);  // Deze methode kan een exception gooien vanuit de domeinlaag
                await _courseRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Skills updated.";  //UX-Polish
                return RedirectToAction(nameof(Details), new { id = dto.Id });
            }

            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.CourseName = courseToUpdate.Name;
                return View(dto);
            }
        }

        [Authorize(Policy = "CourseManagement")]
        [HttpGet("edittimeslots/{id}")]
        public async Task<IActionResult> EditTimeSlots(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            var dto = new UpdateTimeSlotsDTO
            {
                TimeSlots = course.ScheduledTimeSlots.Select(slot => new ScheduledTimeSlotDTO
                {
                    Day = slot.Day,
                    Start = slot.TimeSlot.StartTime,
                    End = slot.TimeSlot.EndTime
                }).ToList()
            };

            ViewBag.CourseName = course.Name;
            ViewBag.CourseId = course.Id;
            return View(dto);
        }

        [Authorize(Policy = "CourseManagement")]
        [HttpPost("edittimeslots/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTimeSlots(int id, UpdateTimeSlotsDTO dto)
        {
            var courseToUpdate = await _courseRepository.GetByIdAsync(id);
            if (courseToUpdate == null)
                return NotFound();

            // Om de view correct opnieuw te tonen bij een fout
            ViewBag.CourseName = courseToUpdate.Name;
            ViewBag.CourseId = courseToUpdate.Id;

            try
            {
                // Converteer DTOs naar domeinobjecten. Dit kan fouten geven als de input ongeldig is.
                var newTimeSlots = dto.TimeSlots.Select(ts =>
                    new ScheduledTimeSlot(ts.Day, new TimeSlot(ts.Start, ts.End))
                ).ToList();

                var currentSlots = courseToUpdate.ScheduledTimeSlots.ToList();
                foreach (var slot in currentSlots)
                {
                    courseToUpdate.RemoveScheduledTimeSlot(slot);
                }

                foreach (var newSlot in newTimeSlots)
                {
                    courseToUpdate.AddScheduledTimeSlot(newSlot);
                }

                await _courseRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Timeslots updated.";
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpPost("confirm/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            try
            {
                course.Confirm(); // Deze methode kan een exception gooien
                await _courseRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Course '{course.Name}' confirmed.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = $" {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpGet("assigncoach/{id}")]
        public async Task<IActionResult> AssignCoach(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            var coaches = await _coachRepository.GetAllAsync();

            var dto = new AssignCoachDTO
            {
                CourseId = course.Id,
                AvailableCoaches = coaches.Select(c => new ListCoaches
                {
                    Id = c.Id,
                    Name = c.Name.ToString()
                }).ToList()
            };

            ViewBag.CourseName = course.Name;
            return View(dto);
        }

        [HttpPost("assigncoach/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCoach(int id, AssignCoachDTO dto)
        {
            if (id != dto.CourseId)
                return BadRequest();

            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            var coach = await _coachRepository.GetByIdAsync(dto.CoachId);
            if (coach == null)
            {
                ModelState.AddModelError("CoachId", "Coach not found.");
                return View(dto);
            }

            try
            {
                course.AssignCoach(coach);  // Zorg dat deze domeinmethode bestaat
                await _courseRepository.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Coach '{coach.Name}' assigned to course.";
                return RedirectToAction(nameof(Details), new { id = course.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                // Herlaad opnieuw de lijst van coaches bij fout
                var coaches = await _coachRepository.GetAllAsync();
                dto.AvailableCoaches = coaches.Select(c => new ListCoaches
                {
                    Id = c.Id,
                    Name = c.Name.ToString()
                }).ToList();

                ViewBag.CourseName = course.Name;
                return View(dto);
            }
        }
    }
}
