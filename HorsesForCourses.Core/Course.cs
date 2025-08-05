namespace HorsesForCourses.Core;

public class Course
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public PlanningPeriod Period { get; private set; }

    private readonly List<string> skills = new();
    public IReadOnlyCollection<string> Skills => skills.AsReadOnly();

    private readonly List<ScheduledTimeSlot> scheduledTimeSlots = new();
    public IReadOnlyCollection<ScheduledTimeSlot> ScheduledTimeSlots => scheduledTimeSlots.AsReadOnly();

    public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public Coach? AssignedCoach { get; private set; }

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
            throw new ArgumentException("Cursusnaam is verplicht.");

        return new Course(name, period);
    }

    public void AddSkill(string skill)
    {
        if (string.IsNullOrWhiteSpace(skill))
            throw new ArgumentException("Competentie kan niet leeg zijn.");

        if (skills.Contains(skill.ToLower()))
            throw new InvalidOperationException("Competentie werd reeds toegevoegd.");

        skills.Add(skill.ToLower());
    }

    public void RemoveSkill(string skill)
    {
        int removed = skills.RemoveAll(c => string.Equals(c, skill.ToLower()));
        if (removed == 0)
            throw new InvalidOperationException($"Competentie '{skill}' niet gevonden.");
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
            throw new InvalidOperationException("Het lesmoment kan niet meer gewijzigd worden na bevestiging of coach toewijzing.");

        if (scheduledTimeSlots.Any(existing => existing.OverlapsWith(slot)))
            throw new InvalidOperationException("Dit lesmoment overlapt met een bestaand lesmoment.");

        scheduledTimeSlots.Add(slot);
    }

    public void RemoveScheduledTimeSlot(ScheduledTimeSlot slot)         //lesmoment verwijderen
    {
        if (Status != CourseStatus.Draft)
            throw new InvalidOperationException("Het lesmoment kan niet meer gewijzigd worden na bevestiging of coach toewijzing.");

        if (!scheduledTimeSlots.Remove(slot))
            throw new InvalidOperationException("Lesmoment niet gevonden.");
    }

    public void Confirm()
    {
        if (!scheduledTimeSlots.Any())
            throw new InvalidOperationException("Kan cursus niet bevestigen zonder lesmoment(en).");

        Status = CourseStatus.Confirmed;
    }

    public void AssignCoach(Coach coach)
    {
        if (Status != CourseStatus.Confirmed)
            throw new InvalidOperationException("Cursus bevestigen voordat je een coach kan toevoegen.");

        if (!coach.HasAllRequiredSkills(skills))
            throw new InvalidOperationException("De coach heeft niet de gewenste competenties voor deze cursus.");

        AssignedCoach = coach;
        Status = CourseStatus.Finalized;
    }
}