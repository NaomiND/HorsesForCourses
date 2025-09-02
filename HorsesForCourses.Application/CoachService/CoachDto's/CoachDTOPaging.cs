using System.ComponentModel.DataAnnotations;

namespace HorsesForCourses.Application.dtos;

public class CoachDTOPaging //zelfde als coachdto ondertussen verwijderd
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; }
    public int NumberOfCoursesAssignedTo { get; set; }              //nodig om aantal courses/coach te tonen

}