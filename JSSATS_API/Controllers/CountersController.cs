using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels;
using JSSATSAPI.BussinessObjects.RequestModels.CounterRequest;
using JSSATSAPI.BussinessObjects.RequestModels.CustomerReqModels;
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

        [HttpPost("create")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateCounters([FromBody] CreateCounteRequest request)
        {
            if (request == null)
            {
                return BadRequest("CreateCounteRequest is null.");
            }

            try
            {
                var response = await _counterService.CreateCounterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Message: {ex.Message}");
            }
        }

        [HttpPut("update/{counterId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateCounterAsync(int counterId, [FromBody] CreateCounteRequest request)
        {
            try
            {
                var updatedProduct = _counterService.UpdateCounterAsync(counterId, request);
                return Ok("Update counter successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
