using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Guild
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("realm")]
        public Realm? Realm { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("faction")]
        public Faction? Faction { get; set; }
    }
}
