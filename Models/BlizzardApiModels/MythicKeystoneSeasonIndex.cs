using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class MythicKeystoneSeasonIndex
    {
        public List<Season> Seasons { get; set; } = default!;

        [JsonPropertyName("current_season")]
        public Season CurrentSeason {get; set;} = default!;
    }
}

