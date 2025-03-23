using System.Text.Json.Serialization;

namespace Singularity.Models.RaiderIoApiModels
{
    public class Region
    {
        public string? Name { get; set; }
        public string? Slug { get; set; } 

        [JsonPropertyName("short_name")]    
        public string? ShortName { get; set; }

    }
}
