using Singularity.Models;
using Singularity.Models.BlizzardApiModels;

namespace Singularity.Services.Interfaces
{
    public interface IBlizzardDataService
    {
        Task<GuildViewModel> GetAllApiData();
        Task GetAccessTokenAsync();
        Task<Roster> GetRosterDataAsync();
        Task<T> GetCachedDataAsync<T>(string endpointKey, Func<Task<T>> apiCall);
        Task<MythicKeystoneSeasonIndex> GetMythicKeystoneSeasonsIndexDataAsync();
    }
}