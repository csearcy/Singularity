namespace Singularity.Models.BlizzardApiModels
 {
    public class MythicKeystoneSeason
    {
        public Season Season { get; set; } = default!;
        public List<Run> BestRuns { get; set; } = default!;
        public Character Character { get; set; } = default!;
        public RatingInfo MythicRating { get; set; } = default!;
    }
}

