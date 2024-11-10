namespace Singularity.Models.BlizzardApiModels
 {
    public class CharacterRaids
    {
        public Character Character { get; set; } = default!;
        public List<ExpansionInfo> Expansions { get; set; } = default!;
    }
}

