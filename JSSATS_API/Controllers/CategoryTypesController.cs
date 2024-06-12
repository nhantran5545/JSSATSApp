using JSSATSAPI.BussinessObjects.IService;
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
    }
}
