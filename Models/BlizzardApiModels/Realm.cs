using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Realm
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }
    }
}
