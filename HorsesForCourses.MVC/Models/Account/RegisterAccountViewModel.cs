namespace HorsesForCourses.MVC.Models;

public class RegisterAccountViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Pass { get; set; } = string.Empty;
    public string ConfirmPass { get; set; } = string.Empty;
    public bool IsCoach { get; set; }
}