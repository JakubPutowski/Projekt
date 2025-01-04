using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using WebApp.Models;

namespace WebApp.Controllers;

public class AccountController : Controller
{
    private static readonly List<AppUser> users = new List<AppUser>
    {
        new AppUser { Email = "adam@wsei.edu.pl", PasswordHash = HashPassword("1234!"), Role = "admin" },
        new AppUser { Email = "natalia@gmail.com", PasswordHash = HashPassword("1234@"), Role = "user" }
    };

    // Strona logowania
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // Obsługa logowania
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        // Znajdź użytkownika w liście
        var hashedPassword = HashPassword(password);
        var user = users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashedPassword);

        if (user != null)
        {
            // Tworzenie tożsamości użytkownika
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Role", user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Logowanie użytkownika
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        // Jeśli logowanie nieudane
        ViewBag.Error = "Invalid email or password";
        return View();
    }
    [Authorize]
    public IActionResult Profile()
    {
        var user = User;
        var profile = new ProfileViewModel
        {
            Email = user.Identity.Name,
            Role = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value
        };

        return View(profile);
    }

    // Wylogowanie użytkownika
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    // Obsługa dostępu zabronionego
    public IActionResult AccessDenied()
    {
        return View();
    }
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
    
}