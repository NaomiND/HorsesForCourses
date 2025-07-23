using System.Data.Common;

namespace horses_for_courses.Core;

public class Course
{
    public Guid Id { get; }
    public string CourseName { get; }
    private readonly List<string> competenceList = new();       //lijst van competenties (collection)
    public IReadOnlyCollection<string> Competences => competenceList.AsReadOnly();

    public Course(Guid id, string course)
    {
        Id = id;
        CourseName = course;
    }
}




// wordt ingepland over een bepaalde periode met een start- en einddatum.
// heeft vaste lesmomenten, bijvoorbeeld op maandag en woensdag van 10u tot 12u.
// heeft enkel les op weekdagen (maandag t.e.m. vrijdag).
// plant lessen uitsluitend binnen de kantooruren (tussen 9u00 en 17u00).
// moet in totaal minstens één uur duren.
// vereist een lijst van coach-competenties.
// wordt begeleid door exact één coach.
// is ongeldig zolang er geen lesmomenten zijn toegevoegd.
// kan pas een coach toegewezen krijgen nadat de opleiding als geldig en definitief is bevestigd.
// laat geen wijzigingen aan het lesrooster meer toe zodra een coach is toegewezen.