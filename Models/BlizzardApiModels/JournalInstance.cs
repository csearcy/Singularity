using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class JournalInstance
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Map Map { get; set; }

        public string Description { get; set; }

        public List<Encounter> Encounters { get; set; }

        public Expansion Expansion { get; set; }

        public Location Location { get; set; }

        public List<ModeParent> Modes { get; set; }

        public Media Media { get; set; }

        [JsonPropertyName("minimum_level")]
        public int MinimumLevel { get; set; }

        public Category Category { get; set; }

        [JsonPropertyName("order_index")]
        public int OrderIndex { get; set; }
    }
}