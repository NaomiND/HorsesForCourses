using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Courses;

namespace HorsesForCourses.Dtos;

public static class CourseMapper
{
    public static CourseDTO ToDTO(Course course)
    {
        return new CourseDTO
        {
            Id = course.Id,
            Name = course.Name.ToString(),
        };
    }

    public static IEnumerable<CourseDTO> ToDTOList(IEnumerable<Course> courses)
    {
        return courses.Select(ToDTO);
    }

    public static CourseDetailDTO ToDTO(Course course, IEnumerable<Coach> coaches)
    {
        return new CourseDetailDTO
        {
            Id = course.Id,
            Name = course.Name.ToString(),
            Skills = course.Skills.ToList(),
            AssignedCoachId = course.AssignedCoach?.Id,
        };
    }

    public static CourseAssignStatusDTO ToAssignmentStatusDTO(Course course)
    {
        return new CourseAssignStatusDTO
        {
            Id = course.Id,
            Name = course.Name,
            StartDate = course.Period.StartDate.ToString(),  // Converteer DateOnly naar DateTime
            EndDate = course.Period.EndDate.ToString(),      // Converteer DateOnly naar DateTime
            HasSchedule = course.ScheduledTimeSlots.Any(),                      // Check of er scheduled time slots zijn
            HasCoach = course.AssignedCoach != null                             // Check of een coach is toegewezen
        };
    }

    public static IEnumerable<CourseAssignStatusDTO> ToAssignmentStatusDTOList(IEnumerable<Course> courses)
    {
        return courses.Select(course => ToAssignmentStatusDTO(course)).ToList();
    }

    public static GetByIdResponse ToGetByIdResponse(Course course)
    {
        return new GetByIdResponse
        {
            Id = course.Id,
            Name = course.Name,
            StartDate = course.Period.StartDate.ToString("yyyy-MM-dd"),
            EndDate = course.Period.EndDate.ToString("yyyy-MM-dd"),
            Skills = course.Skills.ToList(),
            TimeSlots = course.ScheduledTimeSlots
                                .Select(slot => new TimeSlotJasonDTO
                                {
                                    Day = slot.Day,                         // Cast Day naar WeekDays
                                    Start = slot.TimeSlot.StartTime,   // toegang tot TimeSlot, dan StartTime, dan Hour (.hour verwijderd voor int)
                                    End = slot.TimeSlot.EndTime        // toegang tot TimeSlot, dan EndTime, dan Hour
                                })
                                .ToList(),

            // Map de coach informatie alleen als een coach is toegewezen
            Coach = course.AssignedCoach != null                             // Map de coach informatie alleen als een coach is toegewezen
                ? new CoachBasicDTO
                {
                    Id = course.AssignedCoach.Id,
                    Name = course.AssignedCoach.Name.ToString()
                }
                : null                                                      // Als er geen coach is toegewezen, is de 'coach' property null in JSON
        };
    }
}




