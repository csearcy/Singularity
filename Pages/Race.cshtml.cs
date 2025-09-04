using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Singularity.Models;
using Singularity.Services.Interfaces;

namespace Singularity.Pages
{
    public class RaceModel : BasePageModel
    {
        private readonly ILogger<RaceModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly BlizzardApiOptions _blizzardApiOptions;

        public RaceModel(
            ILogger<RaceModel> logger,
            IConfiguration configuration,
            IBlizzardDataService blizzardDataService,
            IRaiderIoDataService raiderIoDataService,
            IOptions<BlizzardApiOptions> blizzardApiOptions)
            : base(blizzardDataService, raiderIoDataService)
        {
            _logger = logger;
            _configuration = configuration;
            _blizzardApiOptions = blizzardApiOptions.Value;
        }

        public string DefaultRaidSlug { get; private set; } 

        public IActionResult OnGet(string? raidName = null)
        {
            DefaultRaidSlug = _blizzardApiOptions.Raids.First(r => r.IsCurrent).BlizzardApiName;
            ViewData["SelectedRaid"] = raidName;
            return Page();
        }

        public async Task<IActionResult> OnGetDataLoading()
        {
            return Partial("_DataLoading");
        }

        public async Task<IActionResult> OnGetRaceTablesPartial(string raidName = null)
        {
            if (RaceViewModel == null || !string.IsNullOrEmpty(raidName))
            {
                await LoadCommonDataAsync(raidName ?? DefaultRaidSlug);
            }

            if (RaceViewModel == null)
            {
                return Partial("_DataLoading");
            }
            
            return Partial("_RaceTables", RaceViewModel);
        }
    }
}