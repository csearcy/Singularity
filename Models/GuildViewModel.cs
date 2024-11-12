using Singularity.Models.BlizzardApiModels;

namespace Singularity.Models
{
    public class GuildViewModel {
        public Roster? Roster { get; set; }
        public MythicKeystoneSeasonIndex? MythicKeystoneSeasonIndex { get; set; }
        public List<MythicKeystoneSeason>? MythicKeystoneSeasons { get; set; }
        public List<CharacterMedia>? CharacterMedias { get; set; }
        public List<Character>? CharacterProfileSummaries { get; set; }
        public List<CharacterRaids>? CharacterRaids { get; set; }

        public enum Roles
        {
            GuildLeader = 0,
            Officer = 1,
            RaidLeader = 3,
            Banker = 4,
            Raider = 6
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