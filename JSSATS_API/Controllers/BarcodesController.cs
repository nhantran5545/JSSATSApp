using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels;
using JSSATSAPI.BussinessObjects.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarcodeController : ControllerBase
    {
        private readonly IBarCodeService _barcodeService;
        private readonly IProductService _productService;

        public BarcodeController(IBarCodeService barcodeService, IProductService productService)
        {
            _barcodeService = barcodeService;
            _productService = productService;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GenerateBarcode(string productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return NotFound(new { Message = "Product not found" });
                }

                var barcode = await _barcodeService.GenerateBarcodeAsync(productId);
                return File(barcode, "image/png");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while generating the barcode", Details = ex.Message });
            }
        }

        [HttpPost("decode")]
        public IActionResult DecodeBarcode(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Invalid file" });
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    var result = _barcodeService.DecodeBarcode(stream);
                    if (string.IsNullOrEmpty(result))
                    {
                        return BadRequest(new { Message = "Unable to decode the barcode" });
                    }
                    return Ok(new { ProductId = result });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while decoding the barcode", Details = ex.Message });
            }
        }
    }

}
