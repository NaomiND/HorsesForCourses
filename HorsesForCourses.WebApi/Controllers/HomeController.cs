using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Application.dtos;
using HorsesForCourses.Infrastructure;
using HorsesForCourses.Core;
using HorsesForCourses.Application.Paging;
using HorsesForCourses.Application.common;
using HorsesForCourses.Application;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get() => Ok("Welcome to the HorsesForCourses API!");
}
