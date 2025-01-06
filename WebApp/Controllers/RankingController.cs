using Microsoft.AspNetCore.Mvc;
using WebApp.Models.University;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers;

public class RankingController : Controller
{
    private readonly UniversityDbContext _context;

    public RankingController(UniversityDbContext context)
    {
        _context = context;
    }

    public IActionResult Details(int universityId, int rankingSystemId)
    {
        // Pobieramy szczegóły rankingu dla uniwersytetu
        var details = _context.UniversityRankingYears
            .Where(ury => ury.UniversityId == universityId && 
                          ury.RankingCriteria.RankingSystemId == rankingSystemId)
            .Select(ury => new RankingDetailsViewModel
            {
                Year = ury.Year ?? 0, // Ustaw 0, jeśli Year jest null
                CriteriaName = ury.RankingCriteria.CriteriaName ?? "Unknown", // Ustaw "Unknown", jeśli CriteriaName jest null
                Score = ury.Score ?? 0 // Ustaw 0, jeśli Score jest null
            })
            .ToList();

        // Pobieramy dane uniwersytetu i systemu rankingowego
        var university = _context.Universities.Find(universityId);
        var rankingSystem = _context.RankingSystems.Find(rankingSystemId);

        ViewBag.UniversityName = university.UniversityName;
        ViewBag.RankingSystemName = rankingSystem.SystemName;

        return View(details);
    }
}
