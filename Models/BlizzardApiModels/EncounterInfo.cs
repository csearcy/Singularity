namespace Singularity.Models.BlizzardApiModels
{
    public class EncounterInfo
    {
        public int CompletedCount { get; set; }
        public Encounter? Encounter { get; set; }
        public long LastKillTimestamp { get; set; }
    }
}
