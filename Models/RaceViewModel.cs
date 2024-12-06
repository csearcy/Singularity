using Singularity.Models.RaiderIoApiModels;

namespace Singularity.Models
{
    public class RaceViewModel {
        public RaidRanking? RaidRankingParent { get; set; }
        public List<BossRanking>? BossRankings { get; set; }
    }
}