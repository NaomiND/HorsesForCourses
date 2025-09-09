using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using HorsesForCourses.MVC.Models;
using HorsesForCourses.Infrastructure;
using Microsoft.AspNetCore.Identity;
using HorsesForCourses.Core;

namespace HorsesForCourses.MVC.AccountController;

[Controller]
[Route("account")]
public class AccountController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountController(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
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
    public async Task<IActionResult> Login(string email)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, email) };
        var id = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(id));
        return Redirect("/");
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

        var passwordHash = _passwordHasher.HashPassword(null, model.Pass);
        var user = new User(model.Name, model.Email, passwordHash);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email) };                            // Automatically sign in the user
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }



    // Automatically sign in the user
    // var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email) };
    // var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    // await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

    // return RedirectToAction("Index", "Home");
}