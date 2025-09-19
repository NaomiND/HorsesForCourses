using HorsesForCourses.Application;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Core;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using static HorsesForCourses.Tests.Mvc.Helper;

namespace HorsesForCourses.Tests.WebApi.Controllers;

public class CoachesControllerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock;
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly CoachesController _controller;

    public CoachesControllerTests()
    {
        _coachRepositoryMock = new Mock<ICoachRepository>();
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _controller = new CoachesController(_coachRepositoryMock.Object, _courseRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithPagedResult()
    {
        var pagedResult = new PagedResult<CoachDTOPaging>(
            Items: new List<CoachDTOPaging> { new() { Id = 1, Name = "Test Coach", Email = "test@coach.com", NumberOfCoursesAssignedTo = 2 } },
            TotalCount: 10,
            PageNumber: 1,
            PageSize: 10
        );
        _coachRepositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<PageRequest>())).ReturnsAsync(pagedResult);

        var result = await _controller.GetAll(new PageRequest());

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDto = Assert.IsType<PagedResult<CoachDTOPaging>>(okResult.Value);
        Assert.Equal(10, returnedDto.TotalCount);
        Assert.Single(returnedDto.Items);
    }

    [Fact]
    public async Task GetCoachById_ReturnsOkWithCoachDetails_WhenCoachExists()
    {
        var coach = Coach.Create("Test Coach", "test@coach.com");
        Hack.TheId(coach, 1);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(coach);

        var result = await _controller.GetCoachById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<CoachDetailDTO>(okResult.Value);
        Assert.Equal(1, returnedDto.Id);
        Assert.Equal("Test Coach", returnedDto.Name);
    }

    [Fact]
    public async Task GetCoachById_ReturnsNotFound_WhenCoachDoesNotExist()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Coach)null);

        var result = await _controller.GetCoachById(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateCoach_ReturnsOkWithNewCoachId()
    {
        var createDto = new CreateCoachDTO { Name = "New Coach", Email = "new@coach.com" };
        var coach = Coach.Create(createDto.Name, createDto.Email);
        Hack.TheId(coach, 10);
        _coachRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Coach>())).Callback<Coach>(c => Hack.TheId(c, 10)).Returns(Task.CompletedTask);
        _coachRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _controller.CreateCoach(createDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(10, okResult.Value);
        _coachRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Coach>()), Times.Once);
        _coachRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCoachSkills_ReturnsNoContent_WhenUpdateIsSuccessful()
    {
        var updateDto = new UpdateCoachSkillsDTO { Id = 1, Skills = new List<string> { "csharp", "azure" } };
        var coach = Coach.Create("Test Coach", "test@coach.com");
        Hack.TheId(coach, 1);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(coach);

        var result = await _controller.UpdateCoachSkills(1, updateDto);

        Assert.IsType<OkObjectResult>(result); // The controller returns Ok(dto)
        _coachRepositoryMock.Verify(r => r.UpdateSkillsAsync(1, updateDto.Skills), Times.Once);
    }

    [Fact]
    public async Task UpdateCoachSkills_ReturnsNotFound_WhenCoachDoesNotExist()
    {
        var updateDto = new UpdateCoachSkillsDTO { Id = 99, Skills = new List<string> { "csharp" } };
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Coach)null);

        var result = await _controller.UpdateCoachSkills(99, updateDto);

        Assert.IsType<NotFoundResult>(result);
    }
}