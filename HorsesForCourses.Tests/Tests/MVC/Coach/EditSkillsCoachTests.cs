using System.Reflection;
using HorsesForCourses.Application;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Core;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.MVC.CoachController;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static HorsesForCourses.Tests.Mvc.Helper;

namespace HorsesForCourses.Tests.Mvc;

public class EditSkillsCoachMVC
{
    private readonly Mock<ICoachRepository> coachRepository;
    private readonly Mock<ICourseRepository> courseRepository;
    private readonly Mock<IUserRepository> userRepository;
    private readonly CoachesController coachController;
    private readonly ControllerTestHelper helper;

    public EditSkillsCoachMVC()
    {
        helper = new ControllerTestHelper();
        coachRepository = helper.CoachRepositoryMock;
        courseRepository = helper.CourseRepositoryMock;
        userRepository = helper.UserRepositoryMock;

        coachController = helper.SetupController(
                    new CoachesController(coachRepository.Object, new Mock<ICourseRepository>().Object, userRepository.Object),
                    TheTester.CoachEmail);
    }

    private Coach SetupTestCoachWithSkills(int coachId, out User user)
    {
        user = User.Create(TheTester.CoachName, TheTester.CoachEmail, "password", new Pbkdf2PasswordHasher());
        Hack.TheId(user, 10);

        var coach = Coach.Create(TheTester.CoachName, TheTester.CoachEmail);
        Hack.TheId(coach, coachId);
        coach.AssignUser(user);

        // Reflection omdat de setter private is.
        var userIdProperty = typeof(Coach).GetProperty(nameof(Coach.UserId));
        userIdProperty?.SetValue(coach, user.Id);

        // Gebruik reflection om de skills uit TheTester toe te voegen aan de coach.
        var skills = TheTester.CoachSkills.Select(s => new Skill(s)).ToList();
        var coachSkills = skills.Select(s => new CoachSkill { Coach = coach, Skill = s }).ToList();
        var coachSkillsField = typeof(Coach).GetField("coachSkills", BindingFlags.NonPublic | BindingFlags.Instance);
        (coachSkillsField?.GetValue(coach) as List<CoachSkill>)?.AddRange(coachSkills);

        userRepository.Setup(r => r.GetByEmailAsync(TheTester.CoachEmail)).ReturnsAsync(user);
        coachRepository.Setup(r => r.GetByIdAsync(coachId)).ReturnsAsync(coach);

        return coach;
    }

    [Fact]
    public async Task EditSkills_GET_ReturnsViewWithSkills_WhenCoachExists()
    {
        var coachId = 5;
        SetupTestCoachWithSkills(coachId, out _);

        var result = await coachController.EditSkills(coachId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateCoachSkillsDTO>(viewResult.Model);
        Assert.Equal(coachId, model.Id);
        Assert.Equal(TheTester.CoachSkills.Select(s => s.ToLower()), model.Skills);
    }

    [Fact]
    public async Task EditSkills_GET_ReturnsNotFound_WhenCoachDoesNotExist()
    {
        coachRepository.Setup(r => r.GetByIdAsync(50)).ReturnsAsync((Coach)null);

        var result = await coachController.EditSkills(50);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task EditSkills_POST_RedirectsToDetails_WhenUpdateIsSuccessful()
    {
        var coachId = 1;
        SetupTestCoachWithSkills(coachId, out _);

        var newSkillsList = new List<string> { "sql", "azure" };
        var dto = new UpdateCoachSkillsDTO { Id = coachId, Skills = newSkillsList };

        var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
             { "Skills", string.Join(",", newSkillsList) }
        });
        coachController.ControllerContext.HttpContext.Request.Form = formCollection;

        var result = await coachController.EditSkills(coachId, dto); ;

        coachRepository.Verify(repo => repo.UpdateSkillsAsync(coachId,
            It.Is<IEnumerable<string>>(skills => skills.SequenceEqual(newSkillsList))), Times.Once);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectToActionResult.ActionName);
        Assert.Equal(coachId, redirectToActionResult.RouteValues["id"]);
    }

    [Fact]
    public async Task EditSkills_POST_ReturnsBadRequest_WhenIdMismatch()
    {
        var dto = new UpdateCoachSkillsDTO { Id = 2, Skills = new List<string> { "C#" } };

        var result = await coachController.EditSkills(1, dto);

        Assert.IsType<BadRequestResult>(result);
    }
}