namespace Singularity.Models.Race
{
    public class RaceViewModel {
        public string SelectedExpansion { get; set; }
        public List<Tuple<string, string>> Raids { get; set; }
        public RealmRaceViewModel RealmRace { get; set; }
        public BossRaceViewModel? BossRace { get; set; }

        public static RaceViewModel ToViewModel(RaceModel raceModel)
        {
            var raceViewModel = new RaceViewModel
            {
                SelectedExpansion = raceModel.SelectedExpansion,
                Raids = raceModel.Raids.Select(raid => new Tuple<string, string>(raid.RaidSlugName, raid.RaidName)).ToList(),
                RealmRace = new RealmRaceViewModel {
                    GuildToIgnoreRank = raceModel.GuildToIgnoreRank,
                    RankingViewModels = raceModel.TopXRealmRaceRanks.OrderBy(s => s.Rank).Select(ranking => new RankingViewModel {
                        TeamName = ranking?.Guild?.Name ?? "TBD",
                        Rank = ranking?.Rank.ToString() ?? "",
                        Faction = ranking?.Guild?.Faction ?? "",
                        EncounterDefeated = ranking?.EncountersDefeated?.Count.ToString() ?? "0",
                        TotalEncounters = raceModel.BossCount.ToString(),
                        Progress = $"{ranking?.EncountersDefeated?.Count ?? 0}/{raceModel.BossCount}"
                    }).ToList() ?? new List<RankingViewModel>()
                },
                BossRace = new BossRaceViewModel {

                } 
            };

            return raceViewModel;
        }
    }
}