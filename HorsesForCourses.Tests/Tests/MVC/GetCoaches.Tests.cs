using HorsesForCourses.MVC;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Infrastructure;
using Moq;
using HorsesForCourses.Application.dtos;
using Microsoft.AspNetCore.Mvc;
using static HorsesForCourses.Tests.Mvc.Helper;

namespace HorsesForCourses.Tests.Mvc;

public class GetCoachesMVC
{
    private readonly Mock<ICoachRepository> coachRepository;
    private readonly Mock<ICourseRepository> courseRepository;
    private readonly CoachesController coachController;

    public GetCoachesMVC()
    {
        coachRepository = new Mock<ICoachRepository>();
        courseRepository = new Mock<ICourseRepository>();
        coachController = new(coachRepository.Object, courseRepository.Object);
    }

    [Fact]
    public async Task GetCoaches_uses_the_query_object()
    {
        var result = await coachController.Index();
        coachRepository.Verify(a => a.GetAllPagedAsync(It.Is<PageRequest>(a => a.PageNumber == 1 && a.PageSize == 10)));
    }

    [Fact]
    public async Task GetCoaches_uses_the_query_object_with_page_info()
    {
        var result = await coachController.Index(3, 8);
        coachRepository.Verify(a => a.GetAllPagedAsync(It.Is<PageRequest>(a => a.PageNumber == 3 && a.PageSize == 8)));
    }

    [Fact]
    public async Task GetCoaches_Passes_The_List_To_The_View()
    {
        var paged = new PagedResult<CoachDTOPaging>(
            new List<CoachDTOPaging>
            { new CoachDTOPaging { Id = 1, Name = TheTester.CoachName, Email = TheTester.CoachEmail, NumberOfCoursesAssignedTo = 0 } }, 1, 1, 10);
        coachRepository.Setup(q => q.GetAllPagedAsync(It.IsAny<PageRequest>())).ReturnsAsync(paged);

        var result = await coachController.Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<PagedResult<CoachDTOPaging>>(view.Model);
        Assert.Single(model.Items);
        Assert.Equal(1, model.TotalCount);
        Assert.Equal(TheTester.CoachName, model.Items[0].Name);
        Assert.Equal(TheTester.CoachEmail, model.Items[0].Email);
        Assert.Equal(1, model.TotalPages);
        Assert.False(model.HasPrevious);
        Assert.False(model.HasNext);
    }
}