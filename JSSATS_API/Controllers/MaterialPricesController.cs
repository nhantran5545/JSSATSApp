using JSSATSAPI.BussinessObjects.IService;
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
        public async Task<IActionResult> GetAllMaterialPrices()
        {
            var materials = await _materialPriceService.GetAllMaterialsAsync();
            if (materials == null || !materials.Any())
            {
                return NotFound("No materials found.");
            }
            return Ok(materials);
        }
    }
}
