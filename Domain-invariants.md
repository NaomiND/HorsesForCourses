1. - De [invarianten](../1.TheStables/readme.md) zoals geïmplementeerd in het domein, dienen door minstens één test gedocumenteerd te worden (meerdere liefst om *edge-cases* te verduidelijken).
   - Een markdown document dient te worden opgeleverd aan de hoofd aandeelhouder via een bestand in de root van de *solution*: `domain-invariants.md`. Daarin voor elke *invariant*:
     - een korte beschrijving 
     - link(s) naar de test code

# Invariants
## EmailAddress
Een e-mailadres mag niet leeg of 'null' zijn en moet een @-teken bevatten als geldig formaat. 

``


FullName.cs
Invarianten: De voor- en achternaam mogen niet leeg zijn. De volledige naam moet minstens uit twee delen bestaan.

Code snippets:

C#

public FullName(string firstName, string lastName)
{
    FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName), "Voornaam verplicht.");
    LastName = lastName ?? throw new ArgumentNullException(nameof(lastName), "Achternaam verplicht.");
}
public static FullName From(string fullName)
{
    if (string.IsNullOrWhiteSpace(fullName))
        throw new ArgumentException("Volledige naam verplicht.", nameof(fullName));
    var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length < 2)
        throw new ArgumentException("Naam moet een voor- en achternaam bevatten.", nameof(fullName));
    return new FullName(parts[0], string.Join(" ", parts.Skip(1)));
}
PlanningPeriod.cs
Invariant: De einddatum moet na de startdatum liggen.

Code snippet:

C#

public PlanningPeriod(DateOnly startDate, DateOnly endDate)
{
    if (endDate <= startDate)
        throw new ArgumentException("Einddatum moet na startdatum liggen.");
    StartDate = startDate;
    EndDate = endDate;
}
ScheduledTimeslot.cs
Invariant: Een lesmoment mag enkel op werkdagen zijn (maandag t.e.m. vrijdag).

Code snippet:

C#

public ScheduledTimeSlot(WeekDays day, TimeSlot timeSlot)
{
    if (!Enum.IsDefined(typeof(WeekDays), day))
        throw new ArgumentException("Ongeldig: enkel lessen van maandag tot vrijdag toegestaan.");
    Day = day;
    TimeSlot = timeSlot;
}
Timeslots.cs
Invarianten: De starttijd moet voor de eindtijd liggen. De les moet minstens 1 uur duren. Het tijdslot moet tussen 09:00 en 17:00 vallen.

Code snippet:

C#

public TimeSlot(int startTime, int endTime)
{
    if (startTime >= endTime)
        throw new ArgumentException("Start moet vroeger zijn dan het einde.");
    if (endTime - startTime < 1)
        throw new ArgumentException("Een les moet minstens 1 uur lang zijn.");
    if (startTime < 9 || endTime > 17)
        throw new ArgumentException("Een tijdslot moet tijdens de kantooruren vallen (09:00-17:00).");
    StartTime = startTime;
    EndTime = endTime;
}
Coach.cs
Invarianten: Een competentie kan niet leeg zijn en mag niet dubbel toegevoegd worden. Een competentie moet aanwezig zijn om te worden verwijderd.

Code snippets:

C#

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
    int removedCount = skills.RemoveAll(c => string.Equals(c, skill.ToLower()));
    if (removedCount == 0)
        throw new InvalidOperationException($"Competentie '{skill}' niet gevonden.");
}