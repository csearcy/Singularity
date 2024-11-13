namespace Singularity.Models.BlizzardApiModels
 {
    public class ExpansionInfo
    {
        public Expansion Expansion { get; set; } = default!;
        public List<InstanceInfo>? Instances { get; set; }
    }
}

