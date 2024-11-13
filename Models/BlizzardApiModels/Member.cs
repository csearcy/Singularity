using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Member
    {
        public Character? Character { get; set; }
        public int Rank { get; set; }
        public Specialization? Specialization { get; set; }
        public Race? Race { get; set; }

        [JsonPropertyName("equipped_item_level")]
        public int EquippedItemLevel { get; set; }

        public string? MythicRating { get; set; }
        public string? RaidProgress { get; set; }

        [JsonIgnore]
        public CharacterMedia Media { get; set; }

        [JsonIgnore]
        public MythicKeystoneSeason MythicKeystoneSeason { get; set; }

        [JsonIgnore]
        public CharacterRaid Raid { get; set; }


    }
}
