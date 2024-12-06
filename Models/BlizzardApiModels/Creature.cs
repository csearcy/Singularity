using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Creature
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        
        [JsonPropertyName("creature_display")]
        public CreatureDisplay CreatureDisplay { get; set; }
    }
}