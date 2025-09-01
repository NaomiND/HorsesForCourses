using HorsesForCourses.Core;

namespace HorsesForCourses.Dtos;

public static class CoachMapper
{
    public static CoachDTO ToDTO(Coach coach, IEnumerable<Course> allCourses)                       // GET Coaches
    {
        int coursesAssigned = allCourses.Count(Courses => Courses.AssignedCoach?.Id == coach.Id);   //tel cursussen waar deze coach is toegewezen

        return new CoachDTO
        {
            Id = coach.Id,
            Name = coach.Name.ToString(),
            Email = coach.Email.Value,
            NumberOfCoursesAssignedTo = coursesAssigned                                             // de berekende waarde
        };
    }

    public static IEnumerable<CoachDTO> ToDTOList(IEnumerable<Coach> coaches, IEnumerable<Course> allCourses)
    {
        return coaches.Select(coach => ToDTO(coach, allCourses)).ToList();
    }

    public static CoachDetailDTO ToDetailDTO(Coach coach, IEnumerable<Course> allCourses)           //GET /coaches/{id} 
    {
        var assignedCourses = allCourses                                    // Filter courses voor deze spec. coach
            .Where(course => course.AssignedCoach?.Id == coach.Id)          // een course, check voor coach ok, vergelijk id
            .Select(course => new CourseDetailDTO                           // match? => new DTO
            {
                Id = course.Id,
                Name = course.Name
            })
            .ToList();

        return new CoachDetailDTO
        {
            Id = coach.Id,
            Name = coach.Name.ToString(),
            Email = coach.Email.Value,
            Skills = coach.Skills.ToList(),
            Courses = assignedCourses
        };
    }
}