using Singularity.Models;

namespace Singularity.Services.Interfaces
{
    public interface IRaiderIoDataService
    {
        Task<RaceViewModel> GetAllApiData();
        Task<bool> GetDataStatusAsync();
    }
}