using Singularity.Models;

namespace Singularity.Services.Interfaces
{
    public interface IRaiderIoDataService
    {
        Task<RaceViewModel> GetAllApiData(List<Boss> bossName);
        Task<bool> GetDataStatusAsync();
    }
}