using horses_for_courses.Core;
namespace horses_for_courses.Dtos;

public class CourseDTO
{
    public Guid Id { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public PlanningPeriod Period { get; set; }
    public IReadOnlyCollection<string> RequiredCompetencies { get; set; } = [];
    public IReadOnlyCollection<ScheduledTimeSlot> ScheduledTimeSlots { get; set; } = [];
    public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public Coach? AssignedCoach { get; private set; }

}