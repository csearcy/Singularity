namespace Singularity.Models.Race
 {
    public class RankingViewModel
    {
        public string TeamName { get; set; } = default!;
        public string Rank { get; set; } = default!;
        public string Faction { get; set; } = default!;
        public string EncounterDefeated { get; set; } = default!;
        public string TotalEncounters { get; set; } = default!;
        public string Progress { get; set; } = default!;
    }
}