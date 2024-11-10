using Singularity.Models.BlizzardApiModels;

namespace Singularity.Services.Interfaces
{
    public interface IBlizzardDataService
    {
        Task GetAccessTokenAsync();
        Task<Roster> GetRosterDataAsync();
        Task<T> GetCachedDataAsync<T>(string endpointKey, Func<Task<T>> apiCall);
        Task<MythicKeystoneSeasonIndex> GetMythicKeystoneSeasonsIndexDataAsync();
    }
}