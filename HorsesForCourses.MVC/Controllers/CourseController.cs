using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;

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
                return View(dto);

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
                ModelState.AddModelError(string.Empty, "Ongeldig formaat. Gebruik jjjj-mm-dd.");
                return View(dto);
            }
        }

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
                TempData["SuccessMessage"] = "Skills zijn succesvol bijgewerkt.";  //UX-Polish
                return RedirectToAction(nameof(Details), new { id = dto.Id });
            }

            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.CourseName = courseToUpdate.Name;
                return View(dto);
            }
        }
    }





}
