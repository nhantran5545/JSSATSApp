using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondPriceResponse;
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
                var payment = await _diamondPriceService.GetAllDiamondPriceAsync();
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
    }
}
