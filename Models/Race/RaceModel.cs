using Singularity.Models.RaiderIoApiModels;

namespace Singularity.Models.Race
{
    public class RaceModel {
        public string SelectedExpansion { get; set; }
        public List<Raid> Raids { get; set; }
        public int GuildToIgnoreRank { get; set; }
        public int BossCount { get; set; }
        public List<Ranking> TopXRealmRaceRanks { get; set; }
        public RaidRanking? RaidRankingParent { get; set; }
        public List<BossRanking>? BossRankings { get; set; }
    }
}