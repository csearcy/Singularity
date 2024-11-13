using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Progress
    {
        [JsonPropertyName("completed_count")]
        public int CompletedCount { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        public List<EncounterInfo>? Encounters { get; set; }
    }
}