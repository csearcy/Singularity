using System.Net;
using Singularity.Models;
using Singularity.Models.BlizzardApiModels;

namespace Singularity.Services.Interfaces
{
    public interface IBlizzardDataService
    {
        Task<bool> GetDataStatusAsync();
        Task<GuildViewModel> PreloadDataAsync(string raidName);
        Task<GuildViewModel> GetCachedDataAsync(string raidName);
    }
}