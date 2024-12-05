using System.Text.Json.Serialization;

namespace Singularity.Models.RaiderIoApiModels
 {
    public class Ranking
    {
        public int Rank { get; set; } = default!;
        public int RegionRank { get; set; } = default!;
        public Guild Guild { get; set; } = default!;
        public List<Encounter> EncountersDefeated { get; set; } = default!;
        public List<Encounter> EncountersPulled { get; set; } = default!;
        public bool? DoesVideoExist { get; set; }

        [JsonPropertyName("itemLevelAvg")]
        public int? ItemLevelAverage { get; set; }
    }
}