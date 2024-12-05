using Singularity.Models.RaiderIoApiModels;

namespace Singularity.Models {
    public class RaiderIoApiOptions
    {
        public string BaseUrl { get; set; } = default!;
        public string Realm { get; set; } = default!;
        public string RaidDifficulty { get; set; } = default!;
        public List<Raid> Raids { get; set; } = default!;
    }
}

