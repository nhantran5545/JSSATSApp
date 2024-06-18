using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.CustomerReqModels;
using JSSATSAPI.BussinessObjects.RequestModels.DiamondRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondPriceResponse;
using JSSATSAPI.BussinessObjects.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondPricesController : ControllerBase
    {
        private readonly IDiamondPriceService _diamondPriceService;

        public DiamondPricesController(IDiamondPriceService diamondPriceService)
        {
            _diamondPriceService = diamondPriceService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DiamondPriceResponse>>> GetDiamondPrice()
        {
            try
            {
                var payment = await _diamondPriceService.GetAllDiamondPrsiceAsync();
                if (payment == null)
                {
                    return NotFound("there are no diamond");
                }

                return Ok(payment);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, "You are not authorized to access this resource.");
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateDiamondPrice([FromBody] DiamondPriceRequest request)
        {
            if (request == null)
            {
                return BadRequest("CustomerRequest is null.");
            }

            try
            {
                var response = await _diamondPriceService.CreateDiamondPriceAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Message: {ex.Message}");
            }
        }

        [HttpPut("{diamondPriceId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateDiamondPrice(int diamondPriceId, [FromBody] UpdateDiamondPriceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _diamondPriceService.UpdateDiamondPriceAsync(diamondPriceId, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Message: {ex.Message}");
            }
        }
    }
}
