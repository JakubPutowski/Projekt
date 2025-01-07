using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Details(int universityId, int rankingSystemId, int pageIndex = 1, int pageSize = 20)
    {
        var source = _context.UniversityRankingYears
            .Where(ury => ury.UniversityId == universityId &&
                          ury.RankingCriteria.RankingSystemId == rankingSystemId)
            .Select(ury => new RankingDetailsViewModel
            {
                Year = ury.Year ?? 0,
                CriteriaName = ury.RankingCriteria.CriteriaName ?? "Unknown", 
                Score = ury.Score ?? 0 
            });
        var paginatedList = await PaginatedList<RankingDetailsViewModel>.CreateAsync(source, pageIndex, pageSize);
    
        var university = _context.Universities.Find(universityId);
        var rankingSystem = _context.RankingSystems.Find(rankingSystemId);
        
        if (!paginatedList.Any())
        {
            ViewBag.NoDataMessage = "Brak danych do wyświetlenia w bazie dla tego rankingu.";
        }
        ViewBag.ReturnUrl = Url.Action("Index", "University");

        ViewBag.UniversityId = universityId;
        ViewBag.RankingSystemId = rankingSystemId;
        ViewBag.UniversityName = university?.UniversityName;
        ViewBag.RankingSystemName = rankingSystem?.SystemName;

        return View(paginatedList);
    }
    [Authorize]
    [HttpGet]
    public IActionResult Create(int universityId, string returnUrl = null)
    {
        var rankingSystems = _context.RankingSystems.ToList();

        ViewBag.UniversityId = universityId;
        ViewBag.RankingSystems = rankingSystems;
        ViewBag.ReturnUrl = returnUrl ?? Url.Action("Details", "Ranking", new { universityId = universityId });
        
        return View();
    }
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UniversityRankingViewModel model, string returnUrl = null)
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
            _context.UniversityRankingYears.Add(rankingYear);
            _context.SaveChanges();
            return Redirect(returnUrl ?? Url.Action("Details", "Ranking", new { universityId = model.UniversityId, rankingSystemId = model.RankingId }));
        }

        ViewBag.UniversityId = model.UniversityId;
        ViewBag.RankingSystems = _context.RankingSystems.ToList();
        ViewBag.Criteria = _context.RankingCriteria.ToList();
        ViewBag.ReturnUrl = returnUrl;

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
