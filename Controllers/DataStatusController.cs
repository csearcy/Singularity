using Microsoft.AspNetCore.Mvc;
using Aphelion.Services.Interfaces;

namespace Aphelion.Controllers
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
    }
}
