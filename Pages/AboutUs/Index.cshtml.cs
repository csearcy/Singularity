using Microsoft.AspNetCore.Mvc;
using Singularity.Models;
using Singularity.Services.Interfaces;

namespace Singularity.Pages.AboutUs
{
    public class IndexModel(ILogger<IndexModel> logger, IBlizzardDataService blizzardDataService) : BasePageModel(blizzardDataService)
    {
        private readonly ILogger<IndexModel> _logger = logger;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCommonDataAsync();
            return Page();
        }
    }
}