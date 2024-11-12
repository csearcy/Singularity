using System.Net;
using Singularity.Models;
using Singularity.Models.BlizzardApiModels;

namespace Singularity.Services.Interfaces
{
    public interface IBlizzardDataService
    {
        Task<GuildViewModel> GetAllApiData();
    }
}