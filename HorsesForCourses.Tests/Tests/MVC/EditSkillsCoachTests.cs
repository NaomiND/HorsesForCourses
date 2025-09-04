using HorsesForCourses.MVC;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Core;
using Moq;
using Microsoft.AspNetCore.Mvc;
using static HorsesForCourses.Tests.Mvc.Helper;
using Microsoft.AspNetCore.Http;
using HorsesForCourses.MVC.CoachController;

namespace HorsesForCourses.Tests.Mvc;

public class EditCoachSkillsMVC
{
    private readonly Mock<ICoachRepository> coachRepository;
    private readonly Mock<ICourseRepository> courseRepository;
    private readonly CoachesController coachController;

    public EditCoachSkillsMVC()
    {
        coachRepository = new Mock<ICoachRepository>();
        courseRepository = new Mock<ICourseRepository>();
        coachController = new(coachRepository.Object, courseRepository.Object);
    }

    [Fact]
    public async Task EditSkills_GET_ReturnsViewWithSkills_WhenCoachExists()
    {
        var coach = new Coach(TheTester.FullName, TheTester.EmailAddress);
        coach.AddSkill("C#");
        coachRepository.Setup(repo => repo.GetByIdAsync(coach.Id)).ReturnsAsync(coach);

        var result = await coachController.EditSkills(coach.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateCoachSkillsDTO>(viewResult.Model);
        Assert.Equal(coach.Id, model.Id);
        Assert.Contains("c#", model.Skills);
        Assert.Equal(coach.Name.DisplayName, viewResult.ViewData["CoachName"]);  //displayname nodig om ze beide als string te bekijken
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
        var coach = new Coach(TheTester.FullName, TheTester.EmailAddress);
        coachRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(coach);

        var updateSkills = new UpdateCoachSkillsDTO { Id = 1, Skills = new List<string> { "c#", "testing" } };

        var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>  // Mocking the HttpContext and Form to handle `Request.Form`
        {{ "Skills", "c#, testing, sql" }});

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Form = formCollection;
        coachController.ControllerContext.HttpContext = httpContext;

        var result = await coachController.EditSkills(1, updateSkills);

        coachRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        Assert.Contains("c#", coach.Skills);
        Assert.Contains("testing", coach.Skills);
        Assert.Contains("sql", coach.Skills);
        Assert.Equal(3, coach.Skills.Count);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectToActionResult.ActionName);
        Assert.Equal(1, redirectToActionResult.RouteValues["id"]);
    }

    [Fact]
    public async Task EditSkills_POST_ReturnsBadRequest_WhenIdMismatch()
    {
        var dto = new UpdateCoachSkillsDTO { Id = 2, Skills = new List<string> { "C#" } };

        var result = await coachController.EditSkills(1, dto);

        Assert.IsType<BadRequestResult>(result);
    }
}