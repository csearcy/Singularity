namespace Singularity.Models.Race
{
    public class RaceViewModel
    {
        public string SelectedExpansion { get; set; }
        public List<Tuple<string, string>> Raids { get; set; }
        public RealmRaceViewModel RealmRace { get; set; }
        public BossRaceViewModel? BossRace { get; set; }

        public static RaceViewModel ToViewModel(RaceModel raceModel)
        {
            try
            {
                var raceViewModel = new RaceViewModel
                {
                    SelectedExpansion = raceModel.SelectedExpansion,
                    Raids = raceModel.Raids.Select(raid => new Tuple<string, string>(raid.RaidSlugName, raid.RaidName)).ToList(),
                    RealmRace = new RealmRaceViewModel
                    {
                        GuildToIgnoreRank = raceModel.GuildToIgnoreRank,
                        RankingViewModels = raceModel.TopXRealmRaceRanks.OrderBy(s => s.Rank).Select(ranking => new RankingViewModel
                        {
                            TeamName = ranking?.Guild?.Name ?? "TBD",
                            Rank = ranking?.Rank.ToString() ?? "",
                            Faction = ranking?.Guild?.Faction ?? "",
                            EncounterDefeated = ranking?.EncountersDefeated?.Count.ToString() ?? "0",
                            TotalEncounters = raceModel.BossCount.ToString(),
                            Progress = $"{ranking?.EncountersDefeated?.Count ?? 0}/{raceModel.BossCount}"
                        }).ToList() ?? new List<RankingViewModel>()
                    },
                    BossRace = new BossRaceViewModel
                    {
                        BossViewModels = raceModel.TopXBossRaceRanks.Select(boss => new BossViewModel
                        {
                            Name = boss?.BossName ?? "TBD",
                            NameSlug = boss?.BossSlugName ?? "TBD",
                            ImageUrl = boss?.BossImageUrl ?? "TBD",
                            RankingViewModels = raceModel.TopXBossRaceRanks.Where(s => s.BossSlugName == boss.BossSlugName).SelectMany(s => s.BossRankings)
                            .Select(ranking => new RankingViewModel
                            {
                                TeamName = ranking?.Guild?.Name ?? "TBD",
                                Rank = ranking?.Rank.ToString() ?? "",
                                Faction = ranking?.Guild?.Faction ?? "",
                                ProgressAsDouble = ranking.EncountersDefeated?.FirstOrDefault(s => s.Slug == GetBossSlug(boss.BossSlugName)) != null ?
                                100 :
                                (ranking?.EncountersPulled?.FirstOrDefault(s => s.Slug == GetBossSlug(boss.BossSlugName))?.IsDefeated) == null ?
                                0 :
                                ranking?.EncountersPulled?.First(s => s.Slug == GetBossSlug(boss.BossSlugName)).BestPercent,
                                Progress = ranking.EncountersDefeated?.FirstOrDefault(s => s.Slug == GetBossSlug(boss.BossSlugName)) != null ?
                                "DEFEATED" : // boss is defeated
                                (ranking?.EncountersPulled?.FirstOrDefault(s => s.Slug == GetBossSlug(boss.BossSlugName))) == null ?
                                "N/A" :  //boss not encountered                              
                                $"{ranking?.EncountersPulled?.First(s => s.Slug == GetBossSlug(boss.BossSlugName)).BestPercent.ToString()}%", //boss encountered, but not defeated
                                FirstDefeatedDate = ranking?.EncountersDefeated?.FirstOrDefault(s => s.Slug == GetBossSlug(boss.BossSlugName))?.FirstDefeated ?? null
                            }).ToList() ?? new List<RankingViewModel>()
                        }).ToList() ?? new List<BossViewModel>()
                    }
                };
                return raceViewModel;
            }
            catch (System.Exception ex)
            {
                return new RaceViewModel();
            }

        }

        public static string GetBossSlug(string bossName)
        {
            var specialSlugs = new Dictionary<string, string>
            {
                { "nexusprincess-kyveza", "nexus-princess-kyveza" },
                { "sikran-captain-of-the-sureki", "sikran" },
                { "the-onearmed-bandit", "onearmed-bandit" },
                { "nexusking-salhadaar", "nexus-king-salhadaar" },
                { "dimensius-the-alldevouring", "dimensius" }
            };

            if (specialSlugs.TryGetValue(bossName, out var slug))
            {
                return slug;
            }

            return bossName;
        }
    }
}