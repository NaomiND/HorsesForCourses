namespace HorsesForCourses.Application.ReadModels;

public class CourseListReadModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public bool HasSchedule { get; set; }
    public bool HasCoach { get; set; }
}