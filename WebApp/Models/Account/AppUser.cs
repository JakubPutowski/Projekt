using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class AppUser
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email {get; set;}
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string PasswordHash {get; set;}
    public string Role {get; set;}
}