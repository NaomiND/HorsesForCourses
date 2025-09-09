using HorsesForCourses.Core;
using HorsesForCourses.MVC.AccountController;
using HorsesForCourses.MVC.Models;
using HorsesForCourses.Tests;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HorsesForCourses.Tests.Mvc;

public class RegisterTests
{
    private readonly ControllerTestHelper helper;
    private readonly AccountController controller;

    public RegisterTests()
    {
        helper = new ControllerTestHelper();

        controller = helper.SetupController(
            new AccountController(helper.UserRepositoryMock.Object, helper.PasswordHasherMock.Object)
        );
    }

    [Fact]
    public async Task Register_ReturnsView_WhenModelStateIsInvalid()
    {
        controller.ModelState.AddModelError("Email", "Required");

        var model = new RegisterAccountViewModel();

        var result = await controller.Register(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public async Task Register_ReturnsView_WhenUserAlreadyExists()
    {
        var model = new RegisterAccountViewModel
        {
            Name = "John",
            Email = "john@example.com",
            Pass = "password123"
        };

        helper.UserRepositoryMock
            .Setup(r => r.GetByEmailAsync(model.Email))
            .ReturnsAsync(new User("Existing", "john@example.com", "hashed"));

        var result = await controller.Register(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.True(controller.ModelState.ContainsKey("Email"));
    }

    [Fact]
    public async Task Register_CreatesUserAndSignsIn_WhenValid()
    {
        var model = new RegisterAccountViewModel
        {
            Name = "Alice",
            Email = "alice@example.com",
            Pass = "securePass"
        };

        helper.UserRepositoryMock
            .Setup(r => r.GetByEmailAsync(model.Email))
            .ReturnsAsync((User)null);

        helper.PasswordHasherMock
            .Setup(p => p.HashPassword(null, model.Pass))
            .Returns("hashedPassword");

        var result = await controller.Register(model);

        helper.UserRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Name == model.Name &&
            u.Email == model.Email &&
            u.PasswordHash == "hashedPassword"
        )), Times.Once);

        helper.UserRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }
}
