namespace Singularity.Models {
    public class Raid
    {
        public bool IsCurrent { get; set; } = default!;
        public string RaiderIoApiName { get; set; } = default!;
        public string BlizzardApiName { get; set; } = default!;
        public string ImageAbbreviation { get; set; } = default!;
    }
}

