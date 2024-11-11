using Microsoft.AspNetCore.Mvc.RazorPages;
using Singularity.Models;
using Singularity.Models.BlizzardApiModels;
using Singularity.Services.Interfaces;

namespace Singularity.Pages {

    public abstract class BasePageModel(IBlizzardDataService blizzardDataService) : PageModel
    {
        private readonly IBlizzardDataService _blizzardDataService = blizzardDataService;
        public GuildViewModel GuildSummary { get; private set; }
       
        protected async Task LoadCommonDataAsync()
        {
            GuildSummary = await _blizzardDataService.GetAllApiData();
        }
    }
}