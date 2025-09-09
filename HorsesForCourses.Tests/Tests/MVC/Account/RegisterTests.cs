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
            new AccountController(helper.UserRepositoryMock.Object, helper.PasswordHasherMock.Object, helper.CoachRepositoryMock.Object)
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
            Name = "Ine De Wit",
            Email = "ine@example.com",
            Pass = "password123",
            ConfirmPass = "password123"
        };

        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("any-hashed-password");

        var existingUser = User.Create("Existing User", model.Email, "any-password", mockHasher.Object);

        helper.UserRepositoryMock
            .Setup(r => r.GetByEmailAsync(model.Email))
            .ReturnsAsync(existingUser);

        var result = await controller.Register(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.True(controller.ModelState.ContainsKey("Email"));
    }


    [Fact]
    public async Task Register_ReturnsView_WhenPasswordIsEmpty()
    {
        var model = new RegisterAccountViewModel { Pass = "", ConfirmPass = "" };

        var result = await controller.Register(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.True(controller.ModelState.ContainsKey("Pass"));
    }


    [Fact]
    public async Task Register_ReturnsView_WhenPasswordsDoNotMatch()
    {
        var model = new RegisterAccountViewModel { Pass = "password123", ConfirmPass = "differentPassword" };

        var result = await controller.Register(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.True(controller.ModelState.ContainsKey("ConfirmPass"));
    }

    [Fact]
    public async Task Register_CreatesUserAndSignsIn_WhenValid()
    {
        var model = new RegisterAccountViewModel
        {
            Name = "Ine De Wit",
            Email = "ine@example.com",
            Pass = "password123",
            ConfirmPass = "password123"
        };

        helper.UserRepositoryMock
            .Setup(r => r.GetByEmailAsync(model.Email))
            .ReturnsAsync((User)null);

        helper.PasswordHasherMock
            .Setup(p => p.Hash(model.Pass))
            .Returns("hashedPassword");

        var result = await controller.Register(model);

        helper.UserRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Name.DisplayName == model.Name &&
            u.Email.Value == model.Email &&
            u.PasswordHash == "hashedPassword"
        )), Times.Once);

        helper.UserRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }
}
