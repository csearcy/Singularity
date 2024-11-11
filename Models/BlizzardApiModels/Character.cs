using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Character
    {
        public int Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public Gender? Gender { get; set; }

        public Faction? Faction { get; set; }

        public Race? Race { get; set; }

        [JsonPropertyName("playable_race")]
        public Race? PlayableRace { get; set; }

        [JsonPropertyName("character_class")]
        public Class CharacterClass { get; set; }

        [JsonPropertyName("playable_class")]
        public Class? PlayableClass { get; set; }

        [JsonPropertyName("active_spec")]
        public Specification ActiveSpec { get; set; }

        public Realm? Realm { get; set; }

        public Guild? Guild { get; set; }

        public int Level { get; set; }

        public int Experience { get; set; }
        
        [JsonPropertyName("achievement_points")]
        public int AchievementPoints { get; set; }

        [JsonPropertyName("average_item_level")]
        public int AverageItemLevel { get; set; }

        [JsonPropertyName("equipped_item_level")]
        public int EquippedItemLevel { get; set; }

        [JsonPropertyName("active_title")]
        public Title? ActiveTitle { get; set; }

        [JsonPropertyName("covenant_progress")]
        public CovenentProgress? CovenentProgress { get; set; }

        [JsonPropertyName("name_search")]
        public string NameSearch { get; set; } = default!;
    }
}
