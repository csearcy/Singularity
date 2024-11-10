namespace Singularity.Models.BlizzardApiModels
 {
    public class CharacterProfileSummary
    {
        public Character Character { get; set; } = default!;
        public List<ExpansionInfo> Expansions { get; set; } = default!;
    }
}

