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
            if (!ModelState.IsValid) return View(dto);

            var coach = new Coach(FullName.From(dto.Name), EmailAddress.Create(dto.Email));
            await _coachRepository.AddAsync(coach);
            await _coachRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
            {
                return BadRequest();
            }

            var coachToUpdate = await _coachRepository.GetByIdAsync(id);
            if (coachToUpdate == null) return NotFound();

            if (ModelState.IsValid)
            {
                // TODO De Skills-lijst komt via de form binding als enkele string, die splitsen we. //dit is wel niet praktisch voor een echte gebruiker
                var skillsList = Request.Form["Skills"].ToString()
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                coachToUpdate.UpdateSkills(skillsList);
                await _coachRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = dto.Id });
            }

            ViewBag.CoachName = coachToUpdate.Name.DisplayName;
            return View(dto);
        }
    }
}