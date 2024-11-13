namespace Singularity.Models.BlizzardApiModels
 {
    public class JournalExpansion
    {
        public List<Tier> Tiers { get; set; } = default!;
    }

    public class Tier {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}

