namespace HorsesForCourses.BlazorWasm.DTOs;

public class CoachDTOPaging
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; }
    public int NumberOfCoursesAssignedTo { get; set; }
}