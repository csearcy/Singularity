namespace Singularity.Models.RaiderIoApiModels
 {
    public class BossRanking
    {
        public string BossName { get; set; } = default!;
        public string BossSlugName { get; set; } = default!;
        public string BossImageUrl { get; set; } = default!;
        public List<Ranking> BossRankings { get; set; } = default!;
    }
}