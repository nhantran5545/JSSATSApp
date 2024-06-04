using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet("allProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var productMaterials = await _productService.GetAllProductMaterialsAsync();
                var productDiamonds = await _productService.GetAllProductDiamondsAsync();

                var allProducts = productMaterials.Concat(productDiamonds).ToList();

                return Ok(allProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("allProductsAvaiable")]
        public async Task<IActionResult> GetAllProductAvaiables()
        {
            try
            {
                var productMaterials = await _productService.GetAllProductMaterialsAvaiableAsync();
                var productDiamonds = await _productService.GetAllProductDiamondsAvaivaleAsync();

                var allProducts = productMaterials.Concat(productDiamonds).ToList();

                return Ok(allProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
