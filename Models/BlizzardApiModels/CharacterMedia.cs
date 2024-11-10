namespace Singularity.Models.BlizzardApiModels
 {
    public class CharacterMedia
    {
        public Character Character { get; set; } = default!;
        public List<Asset> Assets { get; set; } = default!;
    }
}

