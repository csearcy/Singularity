namespace Singularity.Models.BlizzardApiModels
 {
    public class CharacterMedia
    {
        public Character Character { get; set; } = new Character();
        public List<Asset> Assets { get; set; } = [];

        public string? AvatarUrl
        {
            get
            {
                return Assets?.FirstOrDefault(a => a.Key == "avatar")?.Value ?? string.Empty;
            }
        }
    }
}

