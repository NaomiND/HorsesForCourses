using HorsesForCourses.Application;
using HorsesForCourses.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.MVC
{
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
        public async Task<IActionResult> Index([FromQuery] PageRequest request)         // toon gepagineerde lijst van coaches
        {
            // if (request == null)
            // {
            //     request = new PageRequest(1, 20);
            // }

            var pagedCoaches = await _coachRepository.GetAllPagedAsync(request);
            return View(pagedCoaches);                                                // Geef het resultaat door aan de View
        }
    }
}
