using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class Member
    {
        public Character? Character { get; set; }

        public int Rank { get; set; }
    }
}
