using Singularity.Models;
using Singularity.Models.Race;

namespace Singularity.Services.Interfaces
{
    public interface IRaiderIoDataService
    {
        Task<RaceModel> GetAllApiData(List<Boss> bossName);
        Task<bool> GetDataStatusAsync();
    }
}