using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.MaterialReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialsController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MaterialTypeResponse>>> GetAllMaterialsWithTypes()
        {
            var materialTypes = await _materialService.GetAllMaterialsWithTypesAsync();
            return Ok(materialTypes);
        }

        [HttpPost]
        [Route("create-material-with-price")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<MaterialWithPriceResponse>> CreateMaterialWithPrice(MaterialRequest request)
        {
            var response = await _materialService.CreateMaterialWithPriceAsync(request);
            return Ok(response);
        }

        [HttpDelete("{materialId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteMaterial(int materialId)
        {
            try
            {
                await _materialService.DeleteMaterialAsync(materialId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
