using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.MaterialReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondPriceResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialPriceResponse;
using JSSATSAPI.BussinessObjects.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialPricesController : ControllerBase
    {
        private readonly IMaterialPriceService _materialPriceService;

        public MaterialPricesController(IMaterialPriceService materialPriceService)
        {
            _materialPriceService = materialPriceService ?? throw new ArgumentNullException(nameof(_materialPriceService));
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllMaterialTypePrices()
        {
            try
            {
                var materials = await _materialPriceService.GetMaterialTypeWithDetailsAsync();
                if (materials == null)
                {
                    return NotFound("No materials found.");
                }
                return Ok(materials);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPut("{materialPriceId}")]
        [Authorize]
        public async Task<IActionResult> UpdateMaterialPrice(int materialPriceId, [FromBody] UpdateMaterialPriceRequest request)
        {
            try
            {
                await _materialPriceService.UpdateMaterialPriceAsync(materialPriceId, request.BuyPrice, request.SellPrice, request.EffDate);
                return Ok(new { message = "Material price updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
