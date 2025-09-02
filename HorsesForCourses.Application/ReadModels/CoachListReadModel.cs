namespace HorsesForCourses.Application.ReadModels;

public class CoachListReadModel //zelfde als coachpagingdto? dan toch via dto?
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int NumberOfCoursesAssignedTo { get; set; }
}