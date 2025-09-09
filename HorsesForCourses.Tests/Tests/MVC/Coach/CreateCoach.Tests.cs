using HorsesForCourses.Infrastructure;
using HorsesForCourses.Core;
using Moq;
using Microsoft.AspNetCore.Mvc;
using static HorsesForCourses.Tests.Mvc.Helper;
using HorsesForCourses.MVC.CoachController;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HorsesForCourses.Tests.Mvc;

public class CreateCoachMVC
{
    private readonly Mock<ICoachRepository> coachRepository;
    private readonly Mock<ICourseRepository> courseRepository;
    private readonly CoachesController coachController;

    public CreateCoachMVC()
    {
        coachRepository = new Mock<ICoachRepository>();
        courseRepository = new Mock<ICourseRepository>();
        coachController = new(coachRepository.Object, courseRepository.Object);
    }

    [Fact]
    public void Create_GET_ReturnsCreateView()
    {
        var result = coachController.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);                           // default view name "Create"
    }

    [Fact]
    public async Task Create_POST_RedirectsToIndex_WhenModelIsValid()
    {
        var registerCoach = TheTester.RegisterCoach();
        coachRepository.Setup(repo => repo.AddAsync(It.IsAny<Coach>())).Returns(Task.CompletedTask);
        coachRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Set up HttpContext (required for TempData)
        var httpContext = new DefaultHttpContext();
        coachController.ControllerContext.HttpContext = httpContext;

        // Set up TempData
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        coachController.TempData = tempData;

        var result = await coachController.Create(registerCoach);

        coachRepository.Verify(repo => repo.AddAsync(It.IsAny<Coach>()), Times.Once);
        coachRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
    }
}