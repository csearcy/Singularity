using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Faction
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
