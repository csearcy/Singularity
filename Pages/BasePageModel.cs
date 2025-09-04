using Microsoft.AspNetCore.Mvc.RazorPages;
using Singularity.Models;
using Singularity.Models.Race;
using Singularity.Services.Interfaces;

namespace Singularity.Pages {

    public abstract class BasePageModel(IBlizzardDataService blizzardDataService, IRaiderIoDataService raiderIoDataService) : PageModel
    {
        private readonly IBlizzardDataService _blizzardDataService = blizzardDataService;
        private readonly IRaiderIoDataService _raiderIoDataService = raiderIoDataService;
        private readonly BlizzardApiOptions _blizzardSettings;
        public GuildViewModel GuildSummary { get; private set; }
        public RaceViewModel RaceViewModel { get; private set; }
       
        public async Task LoadCommonDataAsync(string raidName = null)
        {
            GuildSummary = await _blizzardDataService.GetCachedDataAsync(raidName);
            RaceViewModel = RaceViewModel.ToViewModel(await _raiderIoDataService.GetCachedDataAsync(GuildSummary.Bosses, raidName));
        }
    }
}