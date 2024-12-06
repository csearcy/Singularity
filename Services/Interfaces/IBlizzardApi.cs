using Refit;
using Singularity.Models.BlizzardApiModels;

namespace Singularity.Services.Interfaces {
    public interface IBlizzardApi
    {
        [Get("/data/wow/guild/{realmSlug}/{guildName}/roster?namespace=profile-us&locale=en_US")]
        Task<Roster> GetRoster(string realmSlug, string guildName, [Header("Authorization")] string authorization);

        [Get("/data/wow/journal-expansion/index?namespace=static-us&locale=en_US")]
        Task<JournalExpansion> GetJournalExpansionData([Header("Authorization")] string authorization);

        [Get("/profile/wow/character/{realmSlug}/{characterName}/mythic-keystone-profile/season/{seasonId}?namespace=profile-us&locale=en_US")]
        Task<MythicKeystoneSeason> GetMythicKeystoneSeasonData(string realmSlug, string characterName, int seasonId, [Header("Authorization")] string authorization);

        [Get("/profile/wow/character/{realmSlug}/{characterName}/character-media?namespace=profile-us&locale=en_US")]
        Task<CharacterMedia> GetCharacterMedia(string realmSlug, string characterName, [Header("Authorization")] string authorization);

        [Get("/profile/wow/character/{realmSlug}/{characterName}?namespace=profile-us&locale=en_US")]
        Task<Character> GetCharacterProfileSummary(string realmSlug, string characterName, [Header("Authorization")] string authorization);

        [Get("/profile/wow/character/{realmSlug}/{characterName}/encounters/raids?namespace=profile-us&locale=en_US")]
        Task<CharacterRaid> GetCharacterRaids(string realmSlug, string characterName, [Header("Authorization")] string authorization);

        [Get("/data/wow/mythic-keystone/season/index?namespace=dynamic-us&locale=en_US")]
        Task<MythicKeystoneSeasonIndex> GetMythicKeystoneSeasonIndex([Header("Authorization")] string authorization);

        [Get("/data/wow/journal-instance/index?namespace=static-us&locale=en_US")]
        Task<JournalSeasonIndex> GetJournalSeasonIndex([Header("Authorization")] string authorization);

        [Get("/data/wow/journal-instance/{journalInstanceId}?namespace=static-us&locale=en_US")]
        Task<JournalInstance> GetJournalInstance(string journalInstanceId, [Header("Authorization")] string authorization);

        [Get("/data/wow/journal-encounter/{encounterId}?namespace=static-us&locale=en_US")]
        Task<JournalEncounter> GetJournalEncounter(string encounterId, [Header("Authorization")] string authorization);

        [Get("/data/wow/media/creature-display/{creatureDisplayId}?namespace=static-us&locale=en_US")]
        Task<CreatureDisplayMedia> GetCreatureDisplayMedia(string creatureDisplayId, [Header("Authorization")] string authorization);
    }
}
