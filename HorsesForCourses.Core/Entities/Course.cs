namespace HorsesForCourses.Core;

public class Course
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public PlanningPeriod Period { get; private set; }
    private readonly List<CourseSkill> _courseSkills = new();
    public IReadOnlyCollection<CourseSkill> CourseSkills => _courseSkills.AsReadOnly();
    private List<ScheduledTimeSlot> scheduledTimeSlots = new();
    public IReadOnlyCollection<ScheduledTimeSlot> ScheduledTimeSlots => scheduledTimeSlots.AsReadOnly();
    public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public Coach? AssignedCoach { get; private set; }
    // private readonly CoachAvailabilityService coachAvailabilityService;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    private Course() { }   // EF Core
#pragma warning restore CS8618

    public Course(string course, PlanningPeriod period)
    {
        Name = course;
        Period = period ?? throw new ArgumentNullException(nameof(period));
    }

    public static Course Create(string name, PlanningPeriod period)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Coursename is required.");

        return new Course(name, period);
    }

    public void AddScheduledTimeSlot(ScheduledTimeSlot slot)            // lesmoment toevoegen
    {
        if (Status != CourseStatus.Draft)
            throw new InvalidOperationException("Timeslot can't be changed after confirmation or coach assignment.");

        if (scheduledTimeSlots.Any(existing => existing.OverlapsWith(slot)))
            throw new InvalidOperationException("This timeslot overlaps with another one.");

        scheduledTimeSlots.Add(slot);
    }

    public void RemoveScheduledTimeSlot(ScheduledTimeSlot slot)         //lesmoment verwijderen
    {
        if (Status != CourseStatus.Draft)
            throw new InvalidOperationException("Timeslot can't be changed after confirmation or coach assignment.");

        if (!scheduledTimeSlots.Remove(slot))
            throw new InvalidOperationException("Timeslot not found.");
    }

    public void Confirm()
    {
        if (!scheduledTimeSlots.Any())
            throw new InvalidOperationException("Confirmation failed. Please add timeslot(s).");

        Status = CourseStatus.Confirmed;
    }

    // public void AssignCoach(Coach coach)
    // {
    //     if (Status != CourseStatus.Confirmed)
    //         throw new InvalidOperationException("Please confirm the course before assigning a coach.");

    //     if (!coach.HasAllRequiredSkills(skills))
    //         throw new InvalidOperationException("This coach does not have the required skills for this course.");

    //     // if (!await coachAvailabilityService.IsCoachAvailableForCourseAsync(coach, this))
    //     // {
    //     //     throw new InvalidOperationException("This coach is unavailable for this course.");
    //     // }
    //     AssignedCoach = coach;
    //     Status = CourseStatus.Finalized;
    // }
}