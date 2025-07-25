using horses_for_courses.WebApi.Controllers;

namespace horses_for_courses.WebApi;

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