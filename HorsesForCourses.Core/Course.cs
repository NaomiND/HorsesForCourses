namespace HorsesForCourses.Core;

public class Course
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public PlanningPeriod Period { get; private set; }
    private List<string> skills = new();
    public IReadOnlyCollection<string> Skills => skills.AsReadOnly();
    private List<ScheduledTimeSlot> scheduledTimeSlots = new();
    public IReadOnlyCollection<ScheduledTimeSlot> ScheduledTimeSlots => scheduledTimeSlots.AsReadOnly();

    public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public Coach? AssignedCoach { get; private set; }
    private readonly CoachAvailabilityService coachAvailabilityService;
    private readonly IEnumerable<Course> allCourses;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    private Course() { }                                         // Private constructor voor EF Core
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

    public void AddSkill(string skill)
    {
        if (string.IsNullOrWhiteSpace(skill))
            throw new ArgumentException("Skill can't be empty.");

        if (skills.Contains(skill.ToLower()))
            throw new InvalidOperationException("Skill already exists.");

        skills.Add(skill.ToLower());
    }

    public void RemoveSkill(string skill)
    {
        int removed = skills.RemoveAll(c => string.Equals(c, skill.ToLower()));
        if (removed == 0)
            throw new InvalidOperationException($"Skill '{skill}' not found.");
    }

    public void ClearSkills()
    {
        skills.Clear();
    }

    public void UpdateSkills(IEnumerable<string> newSkills)
    {
        ClearSkills();

        foreach (var skill in newSkills)
        {
            AddSkill(skill);
        }
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

    public void AssignCoach(Coach coach)
    {
        if (Status != CourseStatus.Confirmed)
            throw new InvalidOperationException("Please confirm the course before assigning a coach.");

        if (!coach.HasAllRequiredSkills(skills))
            throw new InvalidOperationException("This coach does not have the required skills for this course.");

        if (!coachAvailabilityService.IsCoachAvailableForCourse(coach, this, allCourses))
        {
            throw new InvalidOperationException("This coach is unavailable for this course.");
        }
        AssignedCoach = coach;
        Status = CourseStatus.Finalized;
    }
}