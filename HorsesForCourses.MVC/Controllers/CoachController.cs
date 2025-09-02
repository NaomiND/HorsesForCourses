using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;

namespace HorsesForCourses.MVC
{

    [Controller]
    [Route("coaches")]
    public class CoachesController : Controller
    {
        private readonly ICoachRepository _coachRepository;
        private readonly ICourseRepository _courseRepository;

        public CoachesController(ICoachRepository coachrepository, ICourseRepository courseRepository)
        {
            _coachRepository = coachrepository;
            _courseRepository = courseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var request = new PageRequest { PageNumber = page, PageSize = 20 };
            var pagedCoaches = await _coachRepository.GetAllPagedAsync(request);
            return View(pagedCoaches); // View: Views/Coaches/Index.cshtml
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var coach = await _coachRepository.GetByIdAsync(id);
            if (coach == null) return NotFound();

            var allCourses = await _courseRepository.GetAllAsync();
            var coachDetailDto = CoachMapper.ToDetailDTO(coach, allCourses);
            return View(coachDetailDto);
        }
        [HttpGet("create")]     // je hebt get nodig om een post te maken (formulier aanvragen dan invullen en posten)
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email")] CreateCoachDTO dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var coach = new Coach(FullName.From(dto.Name), EmailAddress.Create(dto.Email));
                    await _coachRepository.AddAsync(coach);
                    await _coachRepository.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", $"Fout bij aanmaken coach: {ex.Message}");
                }
            }
            return View(dto);
        }
    }
}