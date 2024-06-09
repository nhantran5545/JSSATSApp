using JSSATSAPI.BussinessObjects.IService;
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
    }
}
