using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountersController : ControllerBase
    {
        private readonly ICounterService _counterService;

        public CountersController(ICounterService counterService)
        {
            _counterService = counterService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCounters()
        {
            try
            {
                var counters = await _counterService.GetAllCounters();
                return Ok(counters);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
