using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
 {
    public class Run
    {
        [JsonPropertyName("completed_timestamp")]
        public int CompletedTimestamp { get; set; } = default!;

        public int Duration { get; set; } = default!;

        public int KeystoneLevel { get; set; } = default!;

        public List<Affixes> KeystoneAffixes { get; set; } = default!;

        public List<Member> Members { get; set; } = default!;

        public Dungeon Dungeon { get; set; } = default!;

        [JsonPropertyName("is_completed_within_time")]
        public bool IsCompletedWithinTime { get; set; } = default!;

        public RatingInfo MythicRating { get; set; } = default!;

        public RatingInfo MapRating { get; set; } = default!;
    }
}