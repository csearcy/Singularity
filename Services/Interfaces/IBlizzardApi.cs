using Refit;
using Singularity.Models.BlizzardApiModels;

namespace Singularity.Services.Interfaces {
    public interface IBlizzardApi
    {
        [Get("/data/wow/guild/{realmSlug}/{guildName}/roster?namespace=profile-us&locale=en_US")]
        Task<Roster> GetRoster(string realmSlug, string guildName, [Header("Authorization")] string authorization);

        [Get("/profile/wow/character/{realmSlug}/{characterName}/mythic-keystone-profile/season/{seasonId}?namespace=profile-us&locale=en_US")]
        Task<MythicKeystoneSeason> GetMythicKeystoneSeasonData(string realmSlug, string characterName, int seasonId, [Header("Authorization")] string authorization);

        [Get("/profile/wow/character/{realmSlug}/{characterName}/character-media?namespace=profile-us&locale=en_US")]
        Task<CharacterMedia> GetCharacterMedia(string realmSlug, string characterName, [Header("Authorization")] string authorization);

        [Get("/profile/wow/character/{realmSlug}/{characterName}?namespace=profile-us&locale=en_US")]
        Task<Character> GetCharacterProfileSummary(string realmSlug, string characterName, [Header("Authorization")] string authorization);

        [Get("/profile/wow/character/{realmSlug}/{characterName}/encounters/raids?namespace=profile-us&locale=en_US")]
        Task<CharacterRaids> GetCharacterRaids(string realmSlug, string characterName, [Header("Authorization")] string authorization);

        [Get("/data/wow/mythic-keystone/season/index?namespace=dynamic-us&locale=en_US")]
        Task<MythicKeystoneSeasonIndex> GetMythicKeystoneSeasonIndex([Header("Authorization")] string authorization);
    }
}
