using System.Text.Json.Serialization;

namespace Singularity.Models.RaiderIoApiModels
{
    public class Realm
    {
        public int Id { get; set; }
        public int ConnectedRealmId { get; set; }
        public int WowRealmId { get; set; }
        public int WowConnectedRealmId { get; set; }
        public string? Name { get; set; }

        [JsonPropertyName("altName")]
        public string? AlternativeName { get; set; }       
        public string? Slug { get; set; }

        [JsonPropertyName("altSlug")]
        public string? AlternativeSlug { get; set; }  

        public string? Locale { get; set; }
        public bool IsConnected { get; set; }
        public string? RealmType { get; set; }

    }
}
