using System.Security.Claims;
using HorsesForCourses.Core;
using HorsesForCourses.MVC.AccountController;
using HorsesForCourses.MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HorsesForCourses.Tests.Mvc;

public class LoginTests
{
    private readonly ControllerTestHelper helper;
    private readonly AccountController controller;

    public LoginTests()
    {
        helper = new ControllerTestHelper();

        controller = helper.SetupController(
            new AccountController(helper.UserRepositoryMock.Object, helper.PasswordHasherMock.Object)
        );
    }

    [Fact]
    public void Login_Get_ReturnsView()
    {
        var result = controller.Login();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Login_Post_SignsInAndRedirects_WhenCredentialsAreValid()
    {
        var model = new LoginViewModel { Email = "test@example.com", Password = "password123" };
        const string knownHash = "a-known-test-hash";
        var userCreationHasherMock = new Mock<IPasswordHasher>();
        userCreationHasherMock.Setup(h => h.Hash(model.Password)).Returns(knownHash);
        var mockUser = User.Create("Test User", model.Email, model.Password, userCreationHasherMock.Object);

        helper.UserRepositoryMock
            .Setup(r => r.GetByEmailAsync(model.Email))
            .ReturnsAsync(mockUser);

        helper.PasswordHasherMock
            .Setup(h => h.Verify(model.Password, knownHash))
            .Returns(true);

        var result = await controller.Login(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);

        helper.AuthServiceMock.Verify(x => x.SignInAsync(
            It.IsAny<HttpContext>(),
            "Cookies",
            It.IsAny<ClaimsPrincipal>(),
            It.IsAny<AuthenticationProperties>()), Times.Once);
    }

    [Fact]
    public async Task Login_Post_ReturnsView_WhenUserNotFound()
    {
        var model = new LoginViewModel { Email = "nouser@example.com", Password = "password123" };
        helper.UserRepositoryMock
            .Setup(r => r.GetByEmailAsync(model.Email))
            .ReturnsAsync((User)null);

        var result = await controller.Login(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(string.Empty));
    }

    [Fact]
    public async Task Login_Post_ReturnsView_WhenPasswordIsInvalid()
    {
        var model = new LoginViewModel { Email = "test@example.com", Password = "wrongpassword" };
        var mockUser = User.Create("Test User", model.Email, "correctpassword", new Infrastructure.Pbkdf2PasswordHasher());

        helper.UserRepositoryMock
            .Setup(r => r.GetByEmailAsync(model.Email))
            .ReturnsAsync(mockUser);

        helper.PasswordHasherMock
            .Setup(h => h.Verify(model.Password, mockUser.PasswordHash))
            .Returns(false);

        var result = await controller.Login(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(string.Empty));
    }

    [Fact]
    public async Task Logout_Post_SignsOutAndRedirects()
    {
        var result = await controller.Logout();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);

        helper.AuthServiceMock.Verify(x => x.SignOutAsync(
            It.IsAny<HttpContext>(),
            "Cookies",
            It.IsAny<AuthenticationProperties>()), Times.Once);
    }
}
