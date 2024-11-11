using Singularity.Models.BlizzardApiModels;

namespace Singularity.Models
{
    public class GuildViewModel {
        public Roster? Roster { get; set; }
        public MythicKeystoneSeasonIndex? MythicKeystoneSeasonIndex { get; set; }
        public MythicKeystoneSeason? MythicKeystoneSeason { get; set; }
        public List<CharacterMedia>? CharacterMedias { get; set; }
        public List<CharacterProfileSummary>? CharacterProfileSummaries { get; set; }
        public List<CharacterRaids>? CharacterRaids { get; set; }

        public enum Roles
        {
            GuildLeader,
            Officer,
            RaidLeader,
            Banker,
            Raider
        }

        public static readonly List<int> AcceptableRoles =
        [
            (int)Roles.GuildLeader,
            (int)Roles.Officer,
            (int)Roles.RaidLeader,
            (int)Roles.Banker,
            (int)Roles.Raider
        ];
    }
}