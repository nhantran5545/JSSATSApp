using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels;
using JSSATSAPI.BussinessObjects.RequestModels.ProductReq;
using JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels;
using JSSATSAPI.BussinessObjects.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        [Authorize]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var allProducts = await _productService.GetAllProductMaterialsAndDiamondsAsync();
/*                var productDiamonds = await _productService.GetAllProductDiamondsAsync();

                var allProducts = productMaterials.Concat(productDiamonds).ToList();*/

                return Ok(allProducts);
            }
            catch (Exception ex)
            {
                return BadRequest($"Message: {ex.Message}");
            }
        }

        [HttpGet("allProductsAvaiable")]
        [Authorize]
        public async Task<IActionResult> GetAllProductAvaiables()
        {
            try
            {
                var productMaterials = await _productService.GetAllProductMaterialsAndDiamondsAvaiableAsync();
                /*var productDiamonds = await _productService.GetAllProductDiamondsAvaivaleAsync();*/

/*                var allProducts = productMaterials.Concat(productDiamonds).ToList();*/

                return Ok(productMaterials);
            }
            catch (Exception ex)
            {
                return BadRequest( $"Message: {ex.Message}");
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateProduct([FromBody] AddProductRequest request)
        {
            if (request == null)
            {
                return BadRequest("ProductRequest is null.");
            }

            try
            {
                var response = await _productService.CreateProductAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Message: {ex.Message}");
            }
        }

        [HttpGet("{productId}")]
        [Authorize]
        public async Task<IActionResult> GetProductById(string productId)
        {
            try
            {
                var products = await _productService.GetProductByIdAsync(productId);
                if (products == null)
                {
                    return NotFound();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync(string productId, [FromBody] UpdateProductRequest request)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(productId, request);
                return Ok(updatedProduct);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Product not found");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
