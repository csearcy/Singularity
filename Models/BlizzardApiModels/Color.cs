using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
 {
    public class Color
    {
        [JsonPropertyName("r")]
        public int Red { get; set; } = default!;

        [JsonPropertyName("g")]
        public int Green { get; set; } = default!;

        [JsonPropertyName("b")]
        public int Blue { get; set; } = default!;

        [JsonPropertyName("a")]
        public double Alpha { get; set; } = default!;
    }
}

