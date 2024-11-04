using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Member
    {
        [JsonPropertyName("character")]
        public Character? Character { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }
    }

    public class Character
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("realm")]
        public Realm? Realm { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("playable_class")]
        public Class? PlayableClass { get; set; }

        [JsonPropertyName("playable_race")]
        public Race? PlayableRace { get; set; }
    }

    public class Class
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class Race
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
