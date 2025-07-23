namespace horses_for_courses.Core;

public enum CourseStatus
{
    Draft,       // Opleiding aangemaakt, nog niet bevestigd
    Confirmed,   // Lesmomenten ok + gevalideerd, klaar voor coach-toewijzing
    Finalized    // Coach toegewezen, verdere wijzigingen niet meer toegestaan
}