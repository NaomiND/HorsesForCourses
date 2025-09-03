using HorsesForCourses.Application.dtos;
using HorsesForCourses.Core;

namespace HorsesForCourses.Tests.Mvc;

public class Helper
{
    public static class TheTester
    {
        public static string CoachName => "Ine De Wit";
        public static string CoachEmail => "Ine.dewit@gmail.com";
        public static List<string> CoachSkills => new List<string> { "C#", "Testing" };
        public static CreateCoachDTO RegisterCoach() => new()
        //toegevoegd voor createcoach - te gebruiken voor live coding? 
        // Tester aanvullen en dan in test gebruiken om herbruikbaarheid te tonen
        {
            Name = CoachName,
            Email = CoachEmail
        };
        public static FullName FullName => FullName.From(CoachName);
        public static EmailAddress EmailAddress => EmailAddress.Create(CoachEmail);
    }

}