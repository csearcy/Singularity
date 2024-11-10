namespace Singularity.Models.BlizzardApiModels
 {
    public class CharacterRaids
    {
        public bool IsCurrent { get; set; } = default!;
        public string RaiderIoApiName { get; set; } = default!;
        public string BlizzardApiName { get; set; } = default!;
        public string ImageAbbreviation { get; set; } = default!;
    }
}

