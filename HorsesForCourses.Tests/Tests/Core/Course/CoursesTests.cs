// using HorsesForCourses.Core;
// namespace HorsesForCourses.Tests;

// public class CourseTests
// {
//     [Fact]
//     public void CreateValidCourse_WithDraftStatus()
//     {
//         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
//         var course = Course.Create("C#", period);

//         Assert.Equal("C#", course.Name);
//         Assert.Equal(CourseStatus.Draft, course.Status);
//     }

//     [Fact]
//     public void AddRequiredSkill_AddsNewToList()
//     {
//         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
//         var course = Course.Create("C#", period);

//         course.AddSkill("Testing");

//         Assert.Contains("testing", course.Skills);
//     }

//     [Theory]
//     [InlineData(null)]
//     [InlineData("")]
//     [InlineData("   ")]
//     public void AddRequiredSkill_InvalidInput_ThrowsArgumentException(string invalidSkill)
//     {
//         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
//         var course = Course.Create("C#", period);

//         Assert.Throws<ArgumentException>(() => course.AddSkill(invalidSkill));
//     }

//     [Fact]
//     public void AddSkill_ThrowsInvalidOperationExceptionWhenSkillAlreadyExists()
//     {
//         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
//         var course = Course.Create("C#", period);
//         string skill = "Dutch";
//         course.AddSkill(skill);

//         Assert.Throws<InvalidOperationException>(() => course.AddSkill("dutch"));
//     }

//     [Fact]
//     public void RemoveRequiredSkill_RemovesExistingSkill()
//     {
//         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
//         var course = Course.Create("C#", period);
//         course.AddSkill("Testing");

//         course.RemoveSkill("testing");

//         Assert.Empty(course.Skills);
//     }

//     [Fact]
//     public void AddScheduledTimeSlot_Valid_AddsSuccessfully()
//     {
//         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
//         var course = Course.Create("C#", period);
//         var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12));
//         // var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(new TimeOnly(10, 0), new TimeOnly(12, 0))); // Voor TimeOnly, maar nu met int start en end time

//         course.AddScheduledTimeSlot(slot);

//         Assert.Single(course.ScheduledTimeSlots);
//     }

//     [Fact]
//     public void Confirm_ChangesStatusToConfirmed()
//     {
//         var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
//         var course = Course.Create("C#", period);
//         var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12));

//         course.AddScheduledTimeSlot(slot);
//         course.Confirm();

//         Assert.Equal(CourseStatus.Confirmed, course.Status);
//     }

//     // [Fact]  // herschijven na aanpassing assign coach met timeslots
//     // public void AssignCoach_Valid_SetCoachChangeStatusToFinalized()
//     // {
//     //     var period = new PlanningPeriod(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31));
//     //     var course = Course.Create("C#", period);
//     //     course.AddSkill("Unit Testing");
//     //     var slot = new ScheduledTimeSlot(WeekDays.Monday, new TimeSlot(10, 12));
//     //     course.AddScheduledTimeSlot(slot);
//     //     course.Confirm();

//     //     var coach = Coach.Create(42, "Coach Test", "test@example.com");
//     //     coach.AddSkill("Unit Testing");

//     //     course.AssignCoach(coach);

//     //     Assert.Equal(CourseStatus.Finalized, course.Status);
//     //     Assert.Equal(coach, course.AssignedCoach);
//     // }
// }