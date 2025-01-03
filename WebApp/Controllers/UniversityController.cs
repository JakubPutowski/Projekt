using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;
public class UniversityController : Controller
{
    private readonly UniversityService _universityService;
    
    public UniversityController(UniversityService universityService)
    {
        _universityService = universityService;
    }
    [HttpGet]
    public IActionResult Index(int pageIndex = 1, int pageSize = 20)
    {
        var universities = _universityService.GetUniverisites(pageIndex, pageSize);
        return View(universities);
    }
}