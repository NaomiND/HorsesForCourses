// using HorsesForCourses.Application;
// using HorsesForCourses.Application.dtos;
// using HorsesForCourses.Core;
// using HorsesForCourses.Infrastructure;
// using HorsesForCourses.MVC.CoachController;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using static HorsesForCourses.Tests.Mvc.Helper;

// namespace HorsesForCourses.Tests.Mvc;

// public class EditCoachSkillsMVC
// {
//     private readonly Mock<ICoachRepository> coachRepository;
//     private readonly Mock<ICourseRepository> courseRepository;
//     private readonly Mock<IUserRepository> userRepository;
//     private readonly CoachesController coachController;
//     private readonly ControllerTestHelper helper;

//     public EditCoachSkillsMVC()
//     {
//         coachRepository = new Mock<ICoachRepository>();
//         courseRepository = new Mock<ICourseRepository>();
//         userRepository = new Mock<IUserRepository>();
//         helper = new ControllerTestHelper();

//         coachController = helper.SetupController(new CoachesController(coachRepository.Object, courseRepository.Object, userRepository.Object), "test-user@example.com");
//     }

//     private User CreateTestUser(int id, string email)
//     {
//         var user = User.Create("Test User", email, "password", new Pbkdf2PasswordHasher());
//         typeof(User).GetProperty(nameof(User.Id))!.SetValue(user, id);
//         return user;
//     }

//     private Coach CreateTestCoach(int coachId, User userToAssign)
//     {
//         var coach = new Coach(TheTester.FullName, TheTester.EmailAddress);

//         coach.AssignUser(userToAssign);
//         typeof(Coach).GetProperty(nameof(Coach.UserId))!.SetValue(coach, userToAssign.Id); // Stel de Foreign Key in

//         typeof(Coach).GetProperty(nameof(Coach.Id))!.SetValue(coach, coachId);
//         return coach;
//     }

//     [Fact]
//     public async Task EditSkills_GET_ReturnsViewWithSkills_WhenCoachExists()
//     {
//         var testUser = CreateTestUser(10, "test-user@example.com");
//         var coach = CreateTestCoach(5, testUser);
//         coach.AddSkill("C#");

//         coachRepository.Setup(repo => repo.GetByIdAsync(5)).ReturnsAsync(coach);
//         userRepository.Setup(repo => repo.GetByEmailAsync("test-user@example.com")).ReturnsAsync(testUser);

//         var result = await coachController.EditSkills(5);

//         var viewResult = Assert.IsType<ViewResult>(result); // Zal nu slagen
//         var model = Assert.IsType<UpdateCoachSkillsDTO>(viewResult.Model);
//         Assert.Equal(5, model.Id);
//     }

//     [Fact]
//     public async Task EditSkills_GET_ReturnsNotFound_WhenCoachDoesNotExist()
//     {
//         coachRepository.Setup(r => r.GetByIdAsync(50)).ReturnsAsync((Coach)null);

//         var result = await coachController.EditSkills(50);

//         Assert.IsType<NotFoundResult>(result);
//     }

//     [Fact]
//     public async Task EditSkills_POST_RedirectsToDetails_WhenUpdateIsSuccessful()
//     {
//         var testUser = CreateTestUser(10, "test-user@example.com");
//         var coach = CreateTestCoach(1, testUser);

//         coachRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(coach);
//         userRepository.Setup(repo => repo.GetByEmailAsync("test-user@example.com")).ReturnsAsync(testUser);

//         var updateSkills = new UpdateCoachSkillsDTO { Id = 1 };
//         var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
//         {
//             { "Skills", "c#, testing, sql" }
//         });
//         coachController.ControllerContext.HttpContext.Request.Form = formCollection;

//         var result = await coachController.EditSkills(1, updateSkills);

//         coachRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once); // Zal nu slagen
//         var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
//         Assert.Equal("Details", redirectToActionResult.ActionName);
//     }

//     [Fact]
//     public async Task EditSkills_POST_ReturnsBadRequest_WhenIdMismatch()
//     {
//         var dto = new UpdateCoachSkillsDTO { Id = 2, Skills = new List<string> { "C#" } };

//         var result = await coachController.EditSkills(1, dto);

//         Assert.IsType<BadRequestResult>(result);
//     }
// }