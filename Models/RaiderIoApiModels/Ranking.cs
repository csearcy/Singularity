using System.Text.Json.Serialization;

namespace Singularity.Models.RaiderIoApiModels
 {
    public class Ranking
    {
        public int Rank { get; set; } = default!;
        public int RegionRank { get; set; } = default!;
        public Guild Guild { get; set; } = default!;
        public List<DefeatedEncounter> EncountersDefeated { get; set; } = default!;

        [JsonPropertyName("encountersPulled")]
        public List<PulledEncounter> EncountersPulled { get; set; } = default!;
        public bool? DoesVideoExist { get; set; }

        [JsonPropertyName("itemLevelAvg")]
        public double? ItemLevelAverage { get; set; }
    }
}