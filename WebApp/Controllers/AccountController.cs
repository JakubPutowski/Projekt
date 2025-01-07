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
        new AppUser { Email = "jakub@wsei.edu.pl", PasswordHash = HashPassword("1234@"), Role = "user" }
    };
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
    {
        var hashedPassword = HashPassword(password);
        var user = users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashedPassword);

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Role", user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        ViewBag.Error = "Invalid email or password";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
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