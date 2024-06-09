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
        public IActionResult GetBarcode(string productId)
        {
            var barcodeImage = _barcodeService.GenerateBarcode(productId);
            return File(barcodeImage, "image/png");
        }

/*        [HttpPost("decode")]
        public IActionResult DecodeBarcode(IFormFile barcodeImageFile)
        {
            if (barcodeImageFile == null || barcodeImageFile.Length == 0)
            {
                return BadRequest("File không được cung cấp.");
            }

            string productId;
            using (var stream = barcodeImageFile.OpenReadStream())
            {
                productId = _barcodeService.DecodeBarcode(stream);
            }

            if (string.IsNullOrEmpty(productId))
            {
                return NotFound("Không thể giải mã mã vạch.");
            }

            var productDetails = _productService.GetProductById(productId);

            if (productDetails == null)
            {
                return NotFound("Không tìm thấy thông tin sản phẩm.");
            }

            return Ok(productDetails);
        }*/

    }
}
