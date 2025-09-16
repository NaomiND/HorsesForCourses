using HorsesForCourses.Core;
namespace HorsesForCourses.Application.dtos;


public class TimeSlotJasonDTO
{
    // Deze property zal worden omgezet naar de string-naam van de enum (bijv. "Monday")
    // dankzij de JsonStringEnumConverter configuratie in Program.cs.
    public WeekDays Day { get; set; }

    // Deze properties zullen de uren als integers bevatten.
    // De mapper zal de conversie van TimeOnly naar int.Hour regelen.
    public int Start { get; set; }
    public int End { get; set; }
}