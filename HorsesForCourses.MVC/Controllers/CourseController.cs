using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;

namespace HorsesForCourses.MVC.CoursController
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

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,StartDate,EndDate")] CreateCourseDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var startDate = DateOnly.Parse(dto.StartDate);
                var endDate = DateOnly.Parse(dto.EndDate);
                var period = new PlanningPeriod(startDate, endDate);

                var course = Course.Create(dto.Name, period);

                await _courseRepository.AddAsync(course);
                await _courseRepository.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cursus is succesvol aangemaakt.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
            catch (FormatException)
            {
                ModelState.AddModelError(string.Empty, "Het formaat van de start- of einddatum is ongeldig. Gebruik jjjj-mm-dd.");
                return View(dto);
            }
        }
    }
}