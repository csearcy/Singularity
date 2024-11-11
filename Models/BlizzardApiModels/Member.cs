using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;

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

        public CharacterMedia Media { get; set; } = new CharacterMedia();
    }
}
