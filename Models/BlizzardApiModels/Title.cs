using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
 {
    public class Title
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        [JsonPropertyName("display_string")]
        public string DisplayString { get; set; } = default!;
    }
}

