namespace Singularity.Models.RaiderIoApiModels
{
    public class Guild
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Realm? Realm { get; set; }
        public string Faction { get; set; }
        public Region Region { get; set; }
        public string? Path { get; set; }
    }
}
