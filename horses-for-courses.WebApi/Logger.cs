using horses_for_courses.WebApi.Controllers;

namespace horses_for_courses.WebApi;

public class Logger
{
    private readonly ILogger<CoachController> _logger;

    public Logger(ILogger<CoachController> logger)
    {
        _logger = logger;
    }
    public void MyLogger()
    {
        _logger.LogInformation("Coach succesvol aangemaakt");
    }
}