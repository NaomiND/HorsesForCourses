using HorsesForCourses.Application;
using HorsesForCourses.Core;
using HorsesForCourses.Dtos;
using HorsesForCourses.Infrastructure;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var request = new PageRequest(page, pageSize);
            var pagedCoaches = await _coachRepository.GetAllPagedAsync(request);
            return View(pagedCoaches); // View: Views/Coaches/Index.cshtml
        }

        // [HttpGet]
        // public async Task<IActionResult> IndexCoach(int page = 1, int pageSize = 20)        // toon gepagineerde lijst van coaches
        // => View(await _coachRepository.GetAllPagedAsync(new PageRequest(page, pageSize)));

    }
}