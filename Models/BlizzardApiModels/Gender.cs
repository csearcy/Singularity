using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Gender
    {
        public string Type { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
