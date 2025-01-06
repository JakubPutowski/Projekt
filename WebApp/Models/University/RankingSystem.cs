using System;
using System.Collections.Generic;

namespace WebApp.Models.University;

public partial class RankingSystem
{
    public int Id { get; set; }

    public string? SystemName { get; set; }

    public ICollection<RankingCriterion> RankingCriteria { get; set; }
}
