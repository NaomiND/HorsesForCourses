using HorsesForCourses.Core;

namespace HorsesForCourses.Application.dtos;

public static class CoachMapper
{
    // De ToDTO methode wordt overbodig, omdat de logica nu in EfCoachRepository zit.
    // Je kunt deze verwijderen of laten staan als je hem elders nog gebruikt.
    // public static CoachDTOPaging ToDTO(Coach coach, IEnumerable<Course> allCourses)                       // GET Coaches
    // {
    //     int coursesAssigned = allCourses.Count(Courses => Courses.AssignedCoach?.Id == coach.Id);   //tel cursussen waar deze coach is toegewezen

    //     return new CoachDTOPaging
    //     {
    //         Id = coach.Id,
    //         Name = coach.Name.ToString(),
    //         Email = coach.Email.Value,
    //         NumberOfCoursesAssignedTo = coursesAssigned                                             // de berekende waarde
    //     };
    // }

    // public static IEnumerable<CoachDTOPaging> ToDTOList(IEnumerable<Coach> coaches, IEnumerable<Course> allCourses)
    // {
    //     return coaches.Select(coach => ToDTO(coach, allCourses)).ToList();
    // }

    public static CoachDetailDTO ToDetailDTO(Coach coach)           //GET /coaches/{id} 
    {
        var assignedCourses = coach.Courses
            .Select(course => new CourseDetailDTO
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