namespace Singularity.Models.RaiderIoApiModels
{
    public class Encounter
    {         
        public string? Slug { get; set; }
        public DateTime? LastDefeated { get; set; }
        public DateTime? FirstDefeated { get; set; }

        public int? Id { get; set; }
        public double? BestPercent { get; set; }
        public bool? IsDefeated { get; set; }
        public int? NumPulls { get; set; }
        public DateTime? PullStartedAt { get; set; }
    }
}
