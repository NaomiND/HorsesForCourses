using HorsesForCourses.Application.dtos;
using HorsesForCourses.Application.Paging;

namespace HorsesForCourses.MVC.ViewModels;

// ViewModel voor de Index-pagina van Coaches
public class CoachIndexViewModel
{
    public PagedResult<CoachDTOPaging>? PagedCoaches { get; set; }
}

