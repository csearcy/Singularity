using System.Text.Json.Serialization;
namespace Singularity.Models.BlizzardApiModels
{
    public class Roster
    {
        public Guild? Guild { get; set; }

        public List<Member>? Members { get; set; }
    }
}