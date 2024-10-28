namespace Singularity.Services.Interfaces
{
    public interface IBlizzardDataService
    {
        Task<string> GetAccessTokenAsync();
        Task<string> GetMembersDataAsync();
        Task<string> GetWowDataAsync(string endpoint);
        Task<string> GetCachedDataAsync(string endpointKey, string apiEndpoint);
    }
}