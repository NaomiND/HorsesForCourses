using HorsesForCourses.Core;

namespace HorsesForCourses.Dtos;

public static class CourseMapper
{
    public static CourseDTO ToDTO(Course course)
    {
        return new CourseDTO
        {
            Id = course.Id,
            Name = course.Name.ToString(),
            Skills = course.Skills.ToList(),
            AssignedCoachId = course.AssignedCoach?.Id,
        };
    }

    public static IEnumerable<CourseDTO> ToDTOList(IEnumerable<Course> courses)
    {
        return courses.Select(ToDTO);
    }
}