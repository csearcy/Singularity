using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Section
    {
        public int? Id { get; set; }
        public string Title { get; set; }

        [JsonPropertyName("body_text")]
        public string BodyText { get; set; }
    }
}
