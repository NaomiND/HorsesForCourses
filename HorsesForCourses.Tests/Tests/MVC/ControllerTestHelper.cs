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
    public Mock<IPasswordHasher> PasswordHasherMock { get; }
    public Mock<IAuthenticationService> AuthServiceMock { get; }
    public T SetupController<T>(T controller) where T : Controller
    {
        var tempDataFactoryMock = new Mock<ITempDataDictionaryFactory>();
        var tempDataProviderMock = new Mock<ITempDataProvider>();

        var httpContext = new DefaultHttpContext();

        var serviceProvider = new ServiceCollection()
            .AddSingleton(AuthServiceMock.Object)
            .AddSingleton<ITempDataDictionaryFactory>(tempDataFactoryMock.Object)
            .AddSingleton<ITempDataProvider>(tempDataProviderMock.Object)
            .BuildServiceProvider();

        httpContext.RequestServices = serviceProvider;

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

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
