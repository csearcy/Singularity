using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
 {
    public class CovenentProgress
    {
        [JsonPropertyName("chosen_covenant")]
        public Covenent ChosenCovenant { get; set; } = default!;

        [JsonPropertyName("renown_level")]
        public int RenownLevel { get; set; } = default!;
    }
}