using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
 {
    public class CovenentProgress
    {
        [JsonPropertyName("chosen_covenant")]
        public ChosenCovenent ChosenCovenant { get; set; } = default!;

        [JsonPropertyName("renown_level")]
        public int RenownLevel { get; set; } = default!;
    }

    public class ChosenCovenent
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}

