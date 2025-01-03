using Microsoft.EntityFrameworkCore;
using WebApp.Models.University;

namespace WebApp.Services;

public class UniversityService
{
    private readonly UniversityDbContext _context;
    
    public UniversityService(UniversityDbContext context)
    {
        _context = context;
    }

    public PaginatedList<University.UniversityDto> GetUniverisites(int pageIndex, int PageSize)
    {
        var query = _context.Universities
            .Include(u => u.Country)
            .Select(u => new University.UniversityDto()
            {
                UniversityName = u.UniversityName,
                CountryName = u.Country.CountryName,
                RankingType1 = "Ranking1",
                RankingType2 = "Ranking2",
                RankingType3 = "Ranking3"
            });
        return PaginatedList<University.UniversityDto>.Create(query, pageIndex, PageSize);
        
    }
}