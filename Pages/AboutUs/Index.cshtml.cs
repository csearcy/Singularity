using Microsoft.AspNetCore.Mvc;
using Singularity.Services.Interfaces;

namespace Singularity.Pages.AboutUs
{
    public class IndexModel(ILogger<IndexModel> logger, IBlizzardDataService blizzardDataService, IRaiderIoDataService raiderIoDataService) : BasePageModel(blizzardDataService, raiderIoDataService)
    {
        private readonly ILogger<IndexModel> _logger = logger;

        public void OnGet()
        {
            _ = LoadCommonDataAsync();
        }

        public IActionResult OnGetDataLoading()
        {
            return Partial("_DataLoading");
        }
    }
}