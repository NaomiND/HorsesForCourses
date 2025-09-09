using System.Security.Claims;
using HorsesForCourses.MVC.AccountController;
using HorsesForCourses.Tests;
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
    public async Task Login_Post_SignsInAndRedirects()
    {
        var email = "test@example.com";

        var result = await controller.Login(email);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/", redirect.Url);

        helper.AuthServiceMock.Verify(x => x.SignInAsync(
            It.IsAny<HttpContext>(),
            "Cookies",
            It.IsAny<ClaimsPrincipal>(),
            It.IsAny<AuthenticationProperties>()), Times.Once);
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
