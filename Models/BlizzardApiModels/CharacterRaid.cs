namespace Singularity.Models.BlizzardApiModels
 {
    public class CharacterRaid
    {
        public Character Character { get; set; } = default!;
        public List<ExpansionInfo> Expansions { get; set; } = default!;
    }
}

