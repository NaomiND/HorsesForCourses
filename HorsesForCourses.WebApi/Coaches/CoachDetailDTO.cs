namespace HorsesForCourses.Dtos;

public class CoachDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; }
    public List<string> Skills { get; set; } = [];
    public int NumberOfCoursesAssignedTo { get; set; }              //nodig om aantal courses/coach te tonen
    public List<CourseDetailDTO> Courses { get; set; } = [];
}