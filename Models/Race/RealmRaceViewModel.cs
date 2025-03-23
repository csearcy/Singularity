namespace Singularity.Models.Race
{
    public class RealmRaceViewModel {
        public int GuildToIgnoreRank { get; set; } = default!;
        public List<RankingViewModel> RankingViewModels { get; set; }
    }
}