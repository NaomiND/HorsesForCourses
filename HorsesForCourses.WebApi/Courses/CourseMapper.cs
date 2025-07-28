using HorsesForCourses.Core;

namespace HorsesForCourses.Dtos;

public static class CourseMapper
{
    public static CourseDTO ToDTO(Course course)
    {
        return new CourseDTO
        {
            Id = course.Id,
            CourseName = course.CourseName.ToString(),
            RequiredCompetences = course.RequiredCompetencies.ToList(),
            AssignedCoachId = course.AssignedCoach?.Id,
        };
    }

    public static IEnumerable<CourseDTO> ToDTOList(IEnumerable<Course> courses)
    {
        return courses.Select(ToDTO);
    }
}