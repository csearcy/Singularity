namespace Singularity.Models.BlizzardApiModels
{
    public class Progress
    {
        public int CompletedCount { get; set; }
        public int TotalCount { get; set; }
        public List<EncounterInfo>? Encounters { get; set; }
    }
}