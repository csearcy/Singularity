namespace Singularity.Models.BlizzardApiModels
{
    public class Guild
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Realm? Realm { get; set; }
        public Faction? Faction { get; set; }
    }
}
