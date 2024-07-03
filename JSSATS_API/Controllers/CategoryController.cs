using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.CategoryRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateCategorys([FromBody] CreateCategoryRequest request)
        {
            if (request == null)
            {
                return BadRequest("CreateCategoryRequest is null.");
            }

            try
            {
                var response = await _categoryService.CreateCategoryAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Message: {ex.Message}");
            }
        }

        [HttpPut("update/{categoryId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateCategoryAsync(int categoryId, [FromBody] CreateCategoryRequest request)
        {
            try
            {
                var counter = await _categoryService.GetCategoryById(categoryId);
                if (counter == null)
                {
                    return NotFound("Category not found");
                }

                if (await _categoryService.UpdateCategoryAsync(categoryId, request))
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
