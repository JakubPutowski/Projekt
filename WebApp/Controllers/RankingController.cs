using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.University;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers;
[Authorize]
public class RankingController : Controller
{
    private readonly UniversityDbContext _context;

    public RankingController(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Details(int universityId, int rankingSystemId, int pageIndex = 1, int pageSize = 20)
    {
        var source = _context.UniversityRankingYears
            .Where(ury => ury.UniversityId == universityId &&
                          ury.RankingCriteria.RankingSystemId == rankingSystemId)
            .Select(ury => new RankingDetailsViewModel
            {
                Year = ury.Year ?? 0,
                CriteriaName = ury.RankingCriteria.CriteriaName ?? "Unknown", // Ustaw "Unknown", jeśli CriteriaName jest null
                Score = ury.Score ?? 0 // Ustaw 0, jeśli Score jest null
            });

        var paginatedList = await PaginatedList<RankingDetailsViewModel>.CreateAsync(source, pageIndex, pageSize);

        // Pobieramy dane uniwersytetu i systemu rankingowego
        var university = _context.Universities.Find(universityId);
        var rankingSystem = _context.RankingSystems.Find(rankingSystemId);

        ViewBag.UniversityId = universityId;
        ViewBag.RankingSystemId = rankingSystemId;
        ViewBag.UniversityName = university.UniversityName;
        ViewBag.RankingSystemName = rankingSystem.SystemName;

        return View(paginatedList);
    }
    [HttpGet]
    public IActionResult Create(int universityId)
    {
        // Pobierz listę systemów rankingowych
        var rankingSystems = _context.RankingSystems.ToList();

        ViewBag.UniversityId = universityId;
        ViewBag.RankingSystems = rankingSystems;

        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UniversityRankingViewModel model)
    {
        if (ModelState.IsValid)
        {
            var rankingYear = new UniversityRankingYear
            {
                UniversityId = model.UniversityId,
                RankingCriteriaId = model.CriterionId,
                Year = model.Year,
                Score = model.Score
            };

            // Dodaj dane do tabeli
            _context.UniversityRankingYears.Add(rankingYear);
            _context.SaveChanges();

            return RedirectToAction("Details", new { universityId = model.UniversityId, rankingSystemId = model.RankingId });
        }

        // Załaduj dane dla nieprawidłowego modelu
        ViewBag.UniversityId = model.UniversityId;
        ViewBag.RankingSystems = _context.RankingSystems.ToList();
        ViewBag.Criteria = _context.RankingCriteria.ToList();

        return View(model);
    }
    [HttpGet]
    public JsonResult GetCriteriaByRanking(int rankingId)
    {
        var criteria = _context.RankingCriteria
            .Where(c => c.RankingSystemId == rankingId)
            .Select(c => new
            {
                c.Id,
                c.CriteriaName
            })
            .ToList();

        return Json(criteria);
    }
}
