using HorsesForCourses.Infrastructure;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Core;
using Moq;
using Microsoft.AspNetCore.Mvc;
using static HorsesForCourses.Tests.Mvc.Helper;
using HorsesForCourses.MVC.CoachController;
using HorsesForCourses.Application;

namespace HorsesForCourses.Tests.Mvc;

public class GetCoachDetailMVC
{
    private readonly Mock<ICoachRepository> coachRepository;
    private readonly Mock<ICourseRepository> courseRepository;
    private readonly Mock<IUserRepository> userRepository;
    private readonly CoachesController coachController;

    public GetCoachDetailMVC()
    {
        coachRepository = new Mock<ICoachRepository>();
        courseRepository = new Mock<ICourseRepository>();
        userRepository = new Mock<IUserRepository>();

        coachController = new(coachRepository.Object, courseRepository.Object, userRepository.Object);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenCoachDoesNotExist()
    {
        coachRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Coach)null);

        var result = await coachController.Details(50);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsViewWithCoachDetails_WhenCoachExists()
    {
        var coach = new Coach(FullName.From(TheTester.CoachName), EmailAddress.Create(TheTester.CoachEmail));
        foreach (var skill in TheTester.CoachSkills)
        {
            coach.AddSkill(skill);
        }

        coachRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(coach);
        courseRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Course>());

        var result = await coachController.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CoachDetailDTO>(viewResult.Model);
        Assert.Equal(TheTester.CoachName, model.Name);
        Assert.Equal(TheTester.CoachEmail, model.Email);
        Assert.NotNull(model.Skills);
        Assert.Equal(TheTester.CoachSkills.Count, model.Skills.Count);
        foreach (var expectedSkill in TheTester.CoachSkills)
        {
            Assert.Contains(expectedSkill.ToLower(), model.Skills);
        }
    }
}
