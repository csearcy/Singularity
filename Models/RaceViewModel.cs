using Singularity.Models.RaiderIoApiModels;

namespace Singularity.Models
{
    public class RaceViewModel {
        public List<Ranking>? RaidRankings { get; set; }
        public List<Ranking>? BossRankings { get; set; }
    }
}