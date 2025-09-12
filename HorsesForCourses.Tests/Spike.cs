// using HorsesForCourses.Core;

// namespace HorsesForCourses.Tests;

// public class Spike
// {
//     [Fact]
//     public void Initial()
//     {
//         var iHavetheDatabaseinMyHand = new HavetheDatabaseInMyHand();

//         var coach = new Coach(FullName.From("Jan Jansen"), EmailAddress.Create("aé@b.c"));
//         var course = new Course("C# Basics", new PlanningPeriod(DateOnly.MinValue, DateOnly.MinValue));
//         var result = No.Go(iHavetheDatabaseinMyHand).FromCoach(coach);
//         Assert.Equal([], result);
//     }

//     [Fact]
//     public void First_no_go()
//     {
//         var iHavetheDatabaseinMyHand = new HavetheDatabaseInMyHand();

//         var coach = new Coach(FullName.From("Jan Jansen"), EmailAddress.Create("aé@b.c"));
//         Hack.TheId(coach, 42);
//         var course = new Course("C# Basics", new PlanningPeriod(DateOnly.MinValue, DateOnly.MinValue));
//         Hack.TheId(coach, 666);
//         course.AddSkill("skill");
//         course.AddScheduledTimeSlot(new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(9, 17)));
//         course.Confirm();
//         var result = No.Go(iHavetheDatabaseinMyHand).FromCoach(coach);
//         Assert.Equal(1, result.Count);
//         Assert.Equal(42, result[0].CoachId);
//         Assert.Equal(666, result[0].CourseId);
//     }
// }
