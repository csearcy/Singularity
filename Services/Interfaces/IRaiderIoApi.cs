using Refit;
using Singularity.Models.RaiderIoApiModels;

namespace Singularity.Services.Interfaces {
    public interface IRaiderIoApi
    {
        [Get("/raiding/raid-rankings?raid={raid}&difficulty={difficulty}&region=us&realm={realmSlug}&page=0")]
        Task<RaidRanking> GetRaidRankings(string raid, string difficulty, string realmSlug);

        [Get("/raiding/boss-rankings?raid={raid}&boss={bossName}&difficulty={difficulty}&region=us&realm={realmSlug}")]
        Task<BossRanking> GetBossRankings(string raid, string difficulty, string bossName, string realmSlug);
    }
}
