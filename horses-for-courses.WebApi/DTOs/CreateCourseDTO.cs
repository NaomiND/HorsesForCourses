namespace horses_for_courses.Dtos;

public class CreateCourseDTO               //Meestal get; set;: standaard en meest flexibele benadering voor DTO's, vanwege serialisatie/deserialisatie.
{
    public Guid Id { get; set; }
    public string CourseName { get; set; }
    // public PlanningPeriod Period { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
//gebruik hier: var dto = CourserMapper.ConvertToDTO(course);
//kan ook gebruikt worden met extensiemethode: public static CourseDTO ConvertToDTO(this course course)
//var dto = course.ConvertToDTO();