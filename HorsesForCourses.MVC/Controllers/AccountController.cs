using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using HorsesForCourses.MVC.Models;
using HorsesForCourses.Infrastructure;
using Microsoft.AspNetCore.Identity;
using HorsesForCourses.Core;
using DomainUser = HorsesForCourses.Core.User;

namespace HorsesForCourses.MVC.AccountController;

[Controller]
[Route("account")]
public class AccountController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AccountController(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    // --- Login/logout actions ---
    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userRepository.GetByEmailAsync(model.Email);

        if (user == null || !_passwordHasher.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
            return View(model);
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email.Value),
        // Je kan hier later meer claims toevoegen, zoals een User ID of rollen
        // new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    };

        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);
        TempData["SuccessMessage"] = "New user registered.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Login");
    }

    // --- Register actions ---
    [AllowAnonymous]
    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterAccountViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Pass))
        {
            ModelState.AddModelError("Pass", "Password is required.");
        }
        else if (model.Pass != model.ConfirmPass)
        {
            ModelState.AddModelError("ConfirmPass", "The passwords don't match.");
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existingUser = await _userRepository.GetByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "An account with this email address already exists.");
            return View(model);
        }

        try
        {
            // var fullName = FullName.From(model.Name);
            // var emailAddress = EmailAddress.Create(model.Email);
            // var passwordHash = _passwordHasher.HashPassword(null, model.Pass);

            var user = DomainUser.Create(model.Name, model.Email, model.Pass, _passwordHasher);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Sign in en redirect logica 
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email.Value) };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }



}
