using HorsesForCourses.Application.dtos;
using HorsesForCourses.MVC;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;




public class GetCoachesMVC
{
    private readonly Mock<ICoachRepository> coachrepository;
    private readonly Mock<ICourseRepository> courseRepository;
    private readonly CoachesController coachController;

    public GetCoachesMVC()
    {
        coachrepository = new Mock<ICoachRepository>();
        courseRepository = new Mock<ICourseRepository>();
        coachController = new(coachrepository.Object, courseRepository.Object);
    }

    [Fact]
    public async Task GetCoaches_uses_the_query_object()
    {
        var result = await coachController.Index();
        //GetAllPagedAsync.Verify(a => a.All(It.Is<PageRequest>(a => a.Page == 1 && a.PageSize == 25)));
    }

    [Fact]
    public async Task GetCoaches_uses_the_query_object_with_page_info()
    {
        //var result = await coachController.Index(3, 15);
        //GetAllPagedAsync.Verify(a => a.All(It.Is<PageRequest>(a => a.Page == 3 && a.PageSize == 15)));
    }

    [Fact]
    public async Task GetCoaches_Passes_The_List_To_The_View()
    {
        // var paged = new PagedResult<CoachDTOPaging>([new(1, TheCanonical.CoachName, TheCanonical.CoachEmail, 0)], 1, 1, 25);
        // GetAllPagedAsync.Setup(q => q.All(It.IsAny<PageRequest>())).ReturnsAsync(paged);

        var result = await coachController.Index();

        // var view = Assert.IsType<ViewResult>(result);
        // var model = Assert.IsType<PagedResult<CoachSummary>>(view.Model);
        // Assert.Single(model.Items);
        // Assert.Equal(1, model.TotalCount);
        // Assert.Equal(TheCanonical.CoachName, model.Items[0].Name);
        // Assert.Equal(TheCanonical.CoachEmail, model.Items[0].Email);
    }
}