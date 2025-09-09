using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Authorization;

namespace HorsesForCourses.MVC.CoachController
{
    [Controller]
    [Route("coaches")]
    [Authorize]
    public class CoachesController : Controller
    {
        private readonly ICoachRepository _coachRepository;
        private readonly ICourseRepository _courseRepository;

        public CoachesController(ICoachRepository coachrepository, ICourseRepository courseRepository)
        {
            _coachRepository = coachrepository;
            _courseRepository = courseRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var request = new PageRequest { PageNumber = page, PageSize = pageSize };
            var pagedCoaches = await _coachRepository.GetAllPagedAsync(request);
            return View(pagedCoaches); // View: Views/Coaches/Index.cshtml
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var coach = await _coachRepository.GetByIdAsync(id);
            if (coach == null)
                return NotFound();

            var coachDetailDto = CoachMapper.ToDetailDTO(coach);
            return View(coachDetailDto);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Email")] CreateCoachDTO dto)
        {
            if (ModelState.IsValid)
                try
                {
                    var coach = new Coach(FullName.From(dto.Name), EmailAddress.Create(dto.Email));
                    await _coachRepository.AddAsync(coach);
                    await _coachRepository.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Coach registered.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            return View(dto);
        }

        [HttpGet("editskills/{id}")]
        public async Task<IActionResult> EditSkills(int id)
        {
            var coach = await _coachRepository.GetByIdAsync(id);
            if (coach == null)
            {
                return NotFound();
            }
            var dto = new UpdateCoachSkillsDTO
            {
                Id = coach.Id,
                Skills = coach.Skills.ToList()
            };
            ViewBag.CoachName = coach.Name.DisplayName;
            return View(dto);
        }

        [HttpPost("editskills/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSkills(int id, [Bind("Id,Skills")] UpdateCoachSkillsDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var coachToUpdate = await _coachRepository.GetByIdAsync(id);
            if (coachToUpdate == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.CoachName = coachToUpdate.Name.DisplayName;
                return View(dto);
            }

            try
            {
                // TODO De Skills-lijst komt via de form binding als enkele string, die splitsen we. //dit is wel niet praktisch voor een echte gebruiker
                var skillsList = Request.Form["Skills"].ToString()
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                coachToUpdate.UpdateSkills(skillsList);  // Deze methode kan een exception gooien vanuit de domeinlaag
                await _coachRepository.SaveChangesAsync();
                TempData["SuccessMessage"] = "Skills updated.";  //UX-Polish
                return RedirectToAction(nameof(Details), new { id = dto.Id });
            }

            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.CoachName = coachToUpdate.Name.DisplayName;
                return View(dto);
            }
        }
    }
}