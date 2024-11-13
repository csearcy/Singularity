using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
 {
    public class MythicKeystoneSeason
    {
        public Season Season { get; set; } = default!;

        [JsonPropertyName("best_runs")]
        public List<Run> BestRuns { get; set; } = default!;
        public Character Character { get; set; } = default!;

        [JsonPropertyName("mythic_rating")]
        public RatingInfo MythicRating { get; set; } = default!;
    }
}

