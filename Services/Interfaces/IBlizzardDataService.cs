using Singularity.Models.BlizzardApiModels;

namespace Singularity.Services.Interfaces
{
    public interface IBlizzardDataService
    {
        Task<string> GetAccessTokenAsync();
        Task<Roster> GetRosterDataAsync();
        Task<string> GetWowDataAsync(string endpoint);
        Task<string> GetCachedDataAsync(string endpointKey, string apiEndpoint);
        Task<string> GetMythicKeystoneSeasonsIndexDataAsync();
    }
}