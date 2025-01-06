using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Models.University;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers;

public class UniversityController : Controller
{
    private readonly UniversityDbContext _context;
    
    public UniversityController(UniversityDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        // Pobieramy dane uniwersytetów i krajów
        var universities = _context.Universities
            .Include(u => u.Country)
            .Select(u => new UniversityViewModel
            {
                UniversityId = u.Id,
                UniversityName = u.UniversityName,
                CountryName = u.Country.CountryName
            })
            .ToList();

        // Pobieramy systemy rankingowe
        ViewBag.RankingSystems = _context.RankingSystems.ToList();

        return View(universities);
    }
    
    
}