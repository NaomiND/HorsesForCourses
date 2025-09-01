using HorsesForCourses.WebApi.Courses;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HorsesForCourses.MVC.ViewModels;

public class AssignCoachViewModel
{
    public GetByIdResponse Course { get; set; }
    public int SelectedCoachId { get; set; }
    public SelectList AvailableCoaches { get; set; }
}