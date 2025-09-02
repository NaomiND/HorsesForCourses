using HorsesForCourses.Application.Paging;
using HorsesForCourses.MVC.ViewModels;
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

        // [HttpGet]
        // public async Task<IActionResult> Index([FromQuery] PageRequest request)
        // {
        //     var pagedCoaches = await _coachRepository.GetAllPagedAsync(request ?? new PageRequest(1, 20));
        //     var viewModel = new CoachIndexViewModel { PagedCoaches = pagedCoaches };
        //     return View(viewModel);
        // }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var request = new PageRequest { PageNumber = page, PageSize = 20 };
            var pagedCoaches = await _coachRepository.GetAllPagedAsync(request);
            return View(pagedCoaches); // View: Views/Coaches/Index.cshtml
        }
    }
}