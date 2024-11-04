using System.Text.Json.Serialization;
namespace Singularity.Models.BlizzardApiModels
{
    public class Roster
    {
        [JsonPropertyName("guild")]
        public Guild? Guild { get; set; }

        [JsonPropertyName("members")]
        public List<Member>? Members { get; set; }
    }
}