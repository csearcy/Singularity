using Microsoft.AspNetCore.Mvc;
using Singularity.Services.Interfaces;

namespace Singularity.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DataStatusController : ControllerBase
    {
        private readonly IBlizzardDataService _blizzardDataService;

        public DataStatusController(IBlizzardDataService blizzardDataService)
        {
            _blizzardDataService = blizzardDataService;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var dataStatus = await _blizzardDataService.GetDataStatusAsync();
            return Ok(new { isReady = dataStatus });
        }
    }
}