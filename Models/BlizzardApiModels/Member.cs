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
        public int EqupippedItemLevel { get; set; }
    }
}
