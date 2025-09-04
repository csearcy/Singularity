using Microsoft.AspNetCore.Mvc;
using Singularity.Services.Interfaces;

namespace Singularity.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataStatusController : ControllerBase
    {
        private readonly IBlizzardDataService _blizzardDataService;
        private readonly IRaiderIoDataService _raiderIoDataService;

        public DataStatusController(IBlizzardDataService blizzardDataService, IRaiderIoDataService raiderIoDataService)
        {
            _blizzardDataService = blizzardDataService;
            _raiderIoDataService = raiderIoDataService;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var blizzardDataStatus = await _blizzardDataService.GetDataStatusAsync();
            var raiderIoDataStatus = await _raiderIoDataService.GetDataStatusAsync();
            var dataStatus = blizzardDataStatus && raiderIoDataStatus;
            return Ok(new { isReady = dataStatus });
        }

        [HttpGet("load")]
        public async Task<IActionResult> LoadRaid([FromQuery] string raidName)
        {
            var guildSummary = await _blizzardDataService.PreloadDataAsync(raidName);
            await _raiderIoDataService.PreloadDataAsync(guildSummary.Bosses, raidName);
            
            return Accepted();
        }
    }
}
