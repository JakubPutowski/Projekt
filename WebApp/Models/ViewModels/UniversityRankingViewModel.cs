using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.ViewModels;

public class UniversityRankingViewModel
{
    [Required]
    public int UniversityId { get; set; }

    [Required(ErrorMessage = "Please select a ranking system.")]
    [Display(Name = "Ranking System")]
    public int RankingId { get; set; } 

    [Required(ErrorMessage = "Please select a criterion.")]
    [Display(Name = "Ranking Criterion")]
    public int CriterionId { get; set; }

    [Required(ErrorMessage = "Year is required.")]
    [Range(2017, int.MaxValue, ErrorMessage = "Year must be above 2016.")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Score is required.")]
    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
    public int Score { get; set; }
}