using HorsesForCourses.Infrastructure;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Core;
using Moq;
using Microsoft.AspNetCore.Mvc;
using static HorsesForCourses.Tests.Mvc.Helper;
using HorsesForCourses.MVC.CoachController;
using HorsesForCourses.Application;
using System.Reflection;

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
        var coach = Coach.Create(TheTester.CoachName, TheTester.CoachEmail);
        Hack.TheId(coach, 1);

        var skills = TheTester.CoachSkills.Select(skillName => new Skill(skillName)).ToList();
        var coachSkills = skills.Select(skill => new CoachSkill { Coach = coach, Skill = skill }).ToList();

        var coachSkillsField = typeof(Coach).GetField("coachSkills", BindingFlags.NonPublic | BindingFlags.Instance);
        (coachSkillsField?.GetValue(coach) as List<CoachSkill>)?.AddRange(coachSkills);

        coachRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(coach);

        var result = await coachController.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CoachDetailDTO>(viewResult.Model);

        Assert.Equal(TheTester.CoachName, model.Name);
        Assert.Equal(TheTester.CoachEmail, model.Email);
        Assert.Equal(TheTester.CoachSkills.Count, model.Skills.Count);
        Assert.Equal(2, model.Skills.Count);
        Assert.Contains("c#", model.Skills);
        Assert.Contains("testing", model.Skills);
    }
}
