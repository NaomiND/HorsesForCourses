// using Microsoft.AspNetCore.Mvc;
// using HorsesForCourses.Infrastructure;
// using HorsesForCourses.Application;
// using HorsesForCourses.Core;
// using HorsesForCourses.Dtos;
// using HorsesForCourses.MVC.ViewModels;
// using Microsoft.AspNetCore.Mvc.Rendering;

// namespace HorsesForCourses.MVC
// {
//     public class CoursesController : Controller
//     {
//         private readonly ICourseRepository _courseRepository;
//         private readonly ICoachRepository _coachRepository;
//         private readonly CoachAvailabilityService _coachAvailabilityService;

//         public CoursesController(ICourseRepository courseRepository, ICoachRepository coachRepository, CoachAvailabilityService coachAvailabilityService)
//         {
//             _courseRepository = courseRepository;
//             _coachRepository = coachRepository;
//             _coachAvailabilityService = coachAvailabilityService;
//         }

//         // GET: /Courses
//         public async Task<IActionResult> Index([FromQuery] PageRequest request)
//         {
//             var pagedCourses = await _courseRepository.GetAllPagedAsync(request ?? new PageRequest(1, 10));
//             return View(pagedCourses);
//         }

//         // GET: /Courses/Details/5
//         public async Task<IActionResult> Details(int id)
//         {
//             var course = await _courseRepository.GetByIdAsync(id);
//             if (course == null)
//             {
//                 return NotFound();
//             }
//             var courseDto = CourseMapper.ToGetByIdResponse(course);
//             return View(courseDto);
//         }

//         // GET: /Courses/AssignCoach/5
//         public async Task<IActionResult> AssignCoach(int id)
//         {
//             var course = await _courseRepository.GetByIdAsync(id);
//             if (course == null) return NotFound();

//             if (course.Status != CourseStatus.Confirmed)
//             {
//                 TempData["Error"] = "Een coach kan enkel toegewezen worden aan een bevestigde cursus.";
//                 return RedirectToAction(nameof(Details), new { id });
//             }

//             var allCoaches = await _coachRepository.GetAllAsync();
//             var availableCoaches = allCoaches
//                 .Where(c => c.HasAllRequiredSkills(course.Skills) &&
//                             _coachAvailabilityService.IsCoachAvailableForCourse(c, course, await _courseRepository.GetAllAsync()))
//                 .ToList();

//             var viewModel = new AssignCoachViewModel
//             {
//                 Course = CourseMapper.ToGetByIdResponse(course),
//                 AvailableCoaches = new SelectList(availableCoaches, "Id", "Name.DisplayName")
//             };

//             return View(viewModel);
//         }

//         // POST: /Courses/AssignCoach/5
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> AssignCoach(int id, [Bind("SelectedCoachId")] AssignCoachViewModel viewModel)
//         {
//             var course = await _courseRepository.GetByIdAsync(id);
//             var coach = await _coachRepository.GetByIdAsync(viewModel.SelectedCoachId);

//             if (course == null || coach == null) return NotFound();

//             try
//             {
//                 course.AssignCoach(coach);
//                 await _courseRepository.SaveChangesAsync();
//                 return RedirectToAction(nameof(Details), new { id });
//             }
//             catch (InvalidOperationException ex)
//             {
//                 TempData["Error"] = ex.Message;
//                 return RedirectToAction(nameof(AssignCoach), new { id });
//             }
//         }
//     }
// }