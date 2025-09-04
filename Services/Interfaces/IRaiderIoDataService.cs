using Singularity.Models;
using Singularity.Models.Race;

namespace Singularity.Services.Interfaces
{
    public interface IRaiderIoDataService
    {
        Task<bool> GetDataStatusAsync();
        Task<RaceModel> PreloadDataAsync(List<Boss> bosses, string raidName);
        Task<RaceModel> GetCachedDataAsync(List<Boss> bosses, string raidName);
    }
}