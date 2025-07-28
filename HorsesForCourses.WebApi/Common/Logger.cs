using HorsesForCourses.WebApi.Controllers;

namespace HorsesForCourses.WebApi;

public class Logger
{
    private readonly ILogger<CoachesController> _logger;

    public Logger(ILogger<CoachesController> logger)
    {
        _logger = logger;
    }
    public void MyLogger()
    {
        _logger.LogInformation("Coach succesvol aangemaakt");
    }
}