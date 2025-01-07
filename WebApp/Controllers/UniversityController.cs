using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models.University;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers;
[Authorize]
public class UniversityController : Controller
{
    private readonly UniversityDbContext _context;
    
    public UniversityController(UniversityDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 20)
    {
        var source = _context.Universities
            .Include(u => u.Country)
            .Select(u => new UniversityViewModel
            {
                UniversityId = u.Id,
                UniversityName = u.UniversityName,
                CountryName = u.Country.CountryName
            });

        var paginatedList = await PaginatedList<UniversityViewModel>.CreateAsync(source, pageIndex, pageSize);

        ViewBag.RankingSystems = _context.RankingSystems.ToList();

        return View(paginatedList);
    }
}