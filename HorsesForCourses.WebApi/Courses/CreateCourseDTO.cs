namespace HorsesForCourses.Dtos;

public class CreateCourseDTO               //Meestal get; set;: standaard en meest flexibele benadering voor DTO's, vanwege serialisatie/deserialisatie.
{
    public string Name { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
}
//gebruik hier: var dto = CourserMapper.ConvertToDTO(course);
//kan ook gebruikt worden met extensiemethode: public static CourseDTO ConvertToDTO(this course course)
//var dto = course.ConvertToDTO();