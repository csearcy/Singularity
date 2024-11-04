using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Singularity.Models.BlizzardApiModels;
using Singularity.Services.Interfaces;

namespace Singularity.Pages.AboutUs
{
    public class IndexModel : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger, IBlizzardDataService blizzardDataService) : base(blizzardDataService)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCommonDataAsync();
            return Page();
        }
    }
}