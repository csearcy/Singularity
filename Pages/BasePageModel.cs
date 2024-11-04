using Microsoft.AspNetCore.Mvc.RazorPages;
using Singularity.Models.BlizzardApiModels;
using Singularity.Services.Interfaces;

namespace Singularity.Pages {

    public abstract class BasePageModel : PageModel
    {
        private readonly IBlizzardDataService _blizzardDataService;


        public Roster RosterData { get; private set; }

        public BasePageModel(IBlizzardDataService blizzardDataService)
        {
            _blizzardDataService = blizzardDataService;
        }

        protected async Task LoadCommonDataAsync()
        {
            RosterData = await _blizzardDataService.GetRosterDataAsync();
        }
    }
}