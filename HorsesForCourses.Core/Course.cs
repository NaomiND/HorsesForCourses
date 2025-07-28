namespace HorsesForCourses.Core;

public class Course
{
    public Guid Id { get; private set; }
    public string CourseName { get; private set; }
    public PlanningPeriod Period { get; private set; }

    private readonly List<string> requiredCompetencies = new();
    public IReadOnlyCollection<string> RequiredCompetencies => requiredCompetencies.AsReadOnly();

    private readonly List<ScheduledTimeSlot> scheduledTimeSlots = new();
    public IReadOnlyCollection<ScheduledTimeSlot> ScheduledTimeSlots => scheduledTimeSlots.AsReadOnly();

    public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public Coach? AssignedCoach { get; private set; }

    public Course(Guid id, string course, PlanningPeriod period)
    {
        Id = id;
        CourseName = course;
        Period = period ?? throw new ArgumentNullException(nameof(period));
    }

    public static Course Create(string name, PlanningPeriod period)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Cursusnaam is verplicht.");

        return new Course(Guid.NewGuid(), name, period);
    }

    public void AddRequiredCompetence(string competency)
    {
        if (string.IsNullOrWhiteSpace(competency))
            throw new ArgumentException("Competentie kan niet leeg zijn.");

        if (requiredCompetencies.Contains(competency.ToLower()))
            throw new InvalidOperationException("Competentie werd reeds toegevoegd.");

        requiredCompetencies.Add(competency.ToLower());
    }

    public void RemoveRequiredCompetence(string competency)
    {
        int removed = requiredCompetencies.RemoveAll(c => string.Equals(c, competency.ToLower()));
        if (removed == 0)
            throw new InvalidOperationException($"Competentie '{competency}' niet gevonden.");
    }

    public void ClearRequiredCompetences()
    {
        requiredCompetencies.Clear();
    }

    public void UpdateRequiredCompetences(IEnumerable<string> newRequiredCompetencies)
    {
        ClearRequiredCompetences();

        foreach (var competence in newRequiredCompetencies)
        {
            AddRequiredCompetence(competence);
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

        if (!coach.HasAllRequiredCompetences(requiredCompetencies))
            throw new InvalidOperationException("De coach heeft niet de gewenste competenties voor deze cursus.");

        AssignedCoach = coach;
        Status = CourseStatus.Finalized;
    }
}