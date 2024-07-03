using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.CategoryRequest;
using JSSATSAPI.BussinessObjects.RequestModels.CounterRequest;
using JSSATSAPI.BussinessObjects.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryTypesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryTypesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("category-types")]
        [Authorize]
        public async Task<IActionResult> GetAllCategoryTypesWithCategories()
        {
            var categoryTypes = await _categoryService.GetAllCategoryTypesWithCategoriesAsync();
            return Ok(categoryTypes);
        }

        [HttpGet("category")]
        [Authorize]
        public async Task<IActionResult> GetAllCategories()
        {
            var categoryTypes = await _categoryService.GetAllCategoriesAsync();
            return Ok(categoryTypes);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateCategoryTypes([FromBody] CreateCategoryTypeRequest request)
        {
            if (request == null)
            {
                return BadRequest("CreateCategoryRequest is null.");
            }

            try
            {
                var response = await _categoryService.CreateCategoryTypeAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Message: {ex.Message}");
            }
        }

        [HttpPut("update/{categoryTypeId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateCategoryTypeAsync(int categoryTypeId, [FromBody] CreateCategoryTypeRequest request)
        {
            try
            {
                var counter = await _categoryService.GetCategoryTypeById(categoryTypeId);
                if (counter == null)
                {
                    return NotFound("Category Type not found");
                }

                if (await _categoryService.UpdateCategoryTypeAsync(categoryTypeId, request))
                {
                    return Ok(request);
                }
                else
                {
                    return BadRequest("Update failed.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Message: {ex.Message}");
            }
        }
    }
}
