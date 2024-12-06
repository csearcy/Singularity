using System.Text.Json.Serialization;

namespace Singularity.Models.BlizzardApiModels
{
    public class JournalEncounter
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<Creature> Creatures { get; set; }

        public List<ItemParent> Items { get; set; }

        public List<Section> Sections { get; set; }

        public Instance Instance { get; set; }

        public Category Category { get; set; }

        public List<Mode> Modes { get; set; }
    }
}