using System.Security.Claims;
using HorsesForCourses.Core;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.MVC.AccountController;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HorsesForCourses.Tests;

public class ControllerTestHelper
{
    public Mock<IUserRepository> UserRepositoryMock { get; }
    public Mock<ICoachRepository> CoachRepositoryMock { get; }
    public Mock<IPasswordHasher> PasswordHasherMock { get; }
    public Mock<IAuthenticationService> AuthServiceMock { get; }
    public T SetupController<T>(T controller, string userEmail = "test@example.com") where T : Controller
    {
        var tempDataFactoryMock = new Mock<ITempDataDictionaryFactory>();
        var tempDataProviderMock = new Mock<ITempDataProvider>();

        var httpContext = new DefaultHttpContext();

        var serviceProvider = new ServiceCollection()
            .AddSingleton(AuthServiceMock.Object)
            // .AddSingleton<ITempDataDictionaryFactory>(tempDataFactoryMock.Object)
            // .AddSingleton<ITempDataProvider>(tempDataProviderMock.Object)
            .BuildServiceProvider();

        httpContext.RequestServices = serviceProvider;

        // Simuleer een ingelogde gebruiker
        if (!string.IsNullOrEmpty(userEmail))
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userEmail),
            }, "mock"));
            httpContext.User = user;
        }

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Initialiseer TempData
        var tempData = new TempDataDictionary(httpContext, tempDataProviderMock.Object);
        controller.TempData = tempData;

        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock
            .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
            .Returns<UrlActionContext>(ctx => $"/{ctx.Action}");

        controller.Url = urlHelperMock.Object;

        return controller;
    }

    public ControllerTestHelper()
    {
        UserRepositoryMock = new Mock<IUserRepository>();
        CoachRepositoryMock = new Mock<ICoachRepository>();
        PasswordHasherMock = new Mock<IPasswordHasher>();
        AuthServiceMock = new Mock<IAuthenticationService>();

        AuthServiceMock
            .Setup(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        AuthServiceMock
            .Setup(x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);
    }
}
