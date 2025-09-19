using HorsesForCourses.Application;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Core;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HorsesForCourses.Tests.WebApi.Controllers;

public class CoursesControllerTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<ICoachRepository> _coachRepositoryMock;
    private readonly Mock<CoachAvailability> _coachAvailabilityMock;
    private readonly CoursesController _controller;

    public CoursesControllerTests()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _coachRepositoryMock = new Mock<ICoachRepository>();
        _coachAvailabilityMock = new Mock<CoachAvailability>(_courseRepositoryMock.Object);
        _controller = new CoursesController(_courseRepositoryMock.Object, _coachRepositoryMock.Object, _coachAvailabilityMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithPagedResult()
    {
        var pagedResult = new PagedResult<CourseAssignStatusDTOPaging>(
            Items: new List<CourseAssignStatusDTOPaging> { new() { Id = 1, Name = "Test Course", Status = "Draft" } },
            TotalCount: 5,
            PageNumber: 1,
            PageSize: 10
        );
        _courseRepositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<PageRequest>())).ReturnsAsync(pagedResult);

        var result = await _controller.GetAll(new PageRequest());

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDto = Assert.IsType<PagedResult<CourseAssignStatusDTOPaging>>(okResult.Value);
        Assert.Equal(5, returnedDto.TotalCount);
        Assert.Single(returnedDto.Items);
    }

    [Fact]
    public async Task GetCourseById_ReturnsOkWithCourseDetails_WhenCourseExists()
    {
        var course = Course.Create("Intro to C#", new PlanningPeriod(new DateOnly(2025, 6, 2), new DateOnly(2025, 9, 19)));
        Hack.TheId(course, 1);
        _courseRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(course);

        var result = await _controller.GetCourseById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<GetByIdResponse>(okResult.Value);
        Assert.Equal(1, returnedDto.Id);
        Assert.Equal("Intro to C#", returnedDto.Name);
    }

    [Fact]
    public async Task GetCourseById_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        _courseRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Course)null);

        var result = await _controller.GetCourseById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateCourse_ReturnsOkWithNewCourseId()
    {
        var createDto = new CreateCourseDTO { Name = "New Course", StartDate = "2025-02-03", EndDate = "2025-02-28" };
        var course = Course.Create(createDto.Name, new PlanningPeriod(DateOnly.Parse(createDto.StartDate), DateOnly.Parse(createDto.EndDate)));
        Hack.TheId(course, 10);
        _courseRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Course>())).Callback<Course>(c => Hack.TheId(c, 10)).Returns(Task.CompletedTask);
        _courseRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _controller.CreateCourse(createDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(10, okResult.Value);
        _courseRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Course>()), Times.Once);
        _courseRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ConfirmCourse_ReturnsNoContent_WhenSuccessful()
    {
        var course = Course.Create("Confirmable Course", new PlanningPeriod(new DateOnly(2025, 2, 3), new DateOnly(2025, 3, 28)));
        course.AddScheduledTimeSlot(new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(9, 11)));
        Hack.TheId(course, 1);
        _courseRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(course);
        _courseRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _controller.ConfirmCourse(1);

        Assert.IsType<NoContentResult>(result);
        _courseRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.Equal(CourseStatus.Confirmed, course.Status);
    }

    [Fact]
    public async Task AssignCoach_ReturnsOk_WhenSuccessful()
    {
        var course = Course.Create("Course to Assign", new PlanningPeriod(new DateOnly(2025, 4, 1), new DateOnly(2025, 4, 10)));
        course.AddScheduledTimeSlot(new ScheduledTimeSlot(WeekDays.Tuesday, new TimeSlot(10, 12)));
        course.Confirm();
        Hack.TheId(course, 1);

        var coach = Coach.Create("Available Coach", "available@coach.com");
        Hack.TheId(coach, 2);

        var skill = new Skill("csharp");
        var coachSkills = new List<CoachSkill> { new CoachSkill { Coach = coach, Skill = skill } };
        typeof(Coach).GetField("coachSkills", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(coach, coachSkills);
        var courseSkills = new List<CourseSkill> { new CourseSkill { Course = course, Skill = skill } };
        typeof(Course).GetField("_courseSkills", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(course, courseSkills);

        var assignDto = new AssignCoachDTO { CourseId = 1, CoachId = 2 };

        _courseRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(course);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(coach);
        _courseRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        _coachAvailabilityMock.Setup(a => a.IsCoachAvailableForCourse(coach, course)).ReturnsAsync(true);

        var result = await _controller.AssignCoach(1, assignDto);

        Assert.IsType<OkObjectResult>(result);
        _courseRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.Equal(CourseStatus.Finalized, course.Status);
        Assert.Equal(coach, course.AssignedCoach);
    }
}