using Singularity.Models.BlizzardApiModels;

namespace Singularity.Models {
    public class BlizzardApiOptions
    {
        public string BaseUrl { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public string TokenEndpoint { get; set; } = default!;
        public GuildSettings Guild { get; set; } = default!;
        public List<Raid> Raids { get; set; } = default!;
    }
}

