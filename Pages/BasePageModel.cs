using Microsoft.AspNetCore.Mvc.RazorPages;
using Singularity.Models;
using Singularity.Models.Race;
using Singularity.Services.Interfaces;

namespace Singularity.Pages {

    public abstract class BasePageModel(IBlizzardDataService blizzardDataService, IRaiderIoDataService raiderIoDataService) : PageModel
    {
        private readonly IBlizzardDataService _blizzardDataService = blizzardDataService;
        private readonly IRaiderIoDataService _raiderIoDataService = raiderIoDataService;
        public GuildViewModel GuildSummary { get; private set; }
        public RaceViewModel RaceViewModel { get; private set; }
       
        protected async Task LoadCommonDataAsync()
        {
            GuildSummary = await _blizzardDataService.GetAllApiData();
            RaceViewModel = RaceViewModel.ToViewModel(await _raiderIoDataService.GetAllApiData(GuildSummary.Bosses));
        }
    }
}