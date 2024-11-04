using Microsoft.AspNetCore.Mvc.RazorPages;
using Singularity.Services.Interfaces;

namespace Singularity.Pages.AboutUs
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IBlizzardDataService _blizzardDataService;        
        
        public IndexModel(ILogger<IndexModel> logger, IBlizzardDataService blizzardDataService)
        {
            _logger = logger;
            _blizzardDataService = blizzardDataService;
        }

        public async Task OnGet()
        {
            var membersData = await _blizzardDataService.GetRosterDataAsync();
            ViewData["RosterData"] = membersData;
        }
    }
}