using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace WebApp.Controllers;

public class AccountController : Controller
{
    private static readonly List<AppUser> _users = new List<AppUser>
    {
        new AppUser { Email = "user@wsei.com", PasswordHash = HashPassword("password"), Role = "User" },
    };
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var HashedPassword = HashPassword(password);
        var user = _users.FirstOrDefault(u => u.Email == email && u.PasswordHash == HashedPassword);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
        };

        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);
        

        return RedirectToAction("Profile");
    }
    
    [HttpGet]
    public IActionResult Profile()
    {
        var email = HttpContext.User.Identity.Name;
        if (email == null)
        {
            return RedirectToAction("Login");
        }
        
        var user = _users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        return View(user);
    }
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
    private static string HashPassword(string password)
    {
        var data = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.Create().ComputeHash(data);
        return Convert.ToBase64String(hash);
    }
      
}
public class AppUser
{
    public string Email { get; set; }
    public string PasswordHash{ get; set; }
    public string Role { get; set; }
}  