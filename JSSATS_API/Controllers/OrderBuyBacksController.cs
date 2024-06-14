using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackResponse;
using JSSATSAPI.BussinessObjects.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderBuyBacksController : ControllerBase
    {
        private readonly IOrderBuyBackService _orderBuyBackService;
        private readonly IProductService _productService;
        private readonly IInvoiceService _invoiceService;

        public OrderBuyBacksController(IOrderBuyBackService orderBuyBackService , IProductService productService , IInvoiceService invoiceService)
        {
            _orderBuyBackService = orderBuyBackService;
            _productService = productService;
            _invoiceService = invoiceService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllOrderBuyBacks()
        {
            try
            {
                var orderSells = await _orderBuyBackService.GetAllOrderBuyBacksAsync();
                return Ok(orderSells);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("BuyBackProductOutOfStore")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> BuyBackProductOutOfStore([FromBody] OrderBuyBackRequest request)
        {
            try
            {
                if (request == null || request.OrderBuyBackDetails == null || !request.OrderBuyBackDetails.Any())
                {
                    return BadRequest("Invalid request.");
                }

                var response = await _orderBuyBackService.BuyBackProductOutOfStoreAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("BuyBackProductInStore")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<OrderBuyBackResponse>> BuyBackInStore(OrderBuyBackInStoreRequest request)
        {
            try
            {
                var response = await _orderBuyBackService.CreateOrderBuyBackInStoreAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("calculate-prices")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> CalculatePrices([FromBody] OrderBuyBackRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var response = await _orderBuyBackService.CalculatePricesAsync(request);

            if (response.Errors != null && response.Errors.Any())
            {
                return BadRequest(new { Errors = response.Errors });
            }

            return Ok(response);
        }

        [HttpPost("review-material-price")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> ReviewMaterialPrice([FromBody] ReviewMaterialPriceRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var response = await _orderBuyBackService.ReviewMaterialPriceAsync(request.MaterialId, request.Weight);

            if (!response.Success)
            {
                return BadRequest(new { ErrorMessage = response.ErrorMessage });
            }

            return Ok(response);
        }

        [HttpPost("review-diamond-price")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> ReviewDiamondPrice([FromBody] ReviewDiamondPriceRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var response = await _orderBuyBackService.ReviewDiamondPriceAsync(request.Origin, request.CaratWeight, request.Color, request.Clarity, request.Cut);

            if (!response.Success)
            {
                return BadRequest(new { ErrorMessage = response.ErrorMessage });
            }

            return Ok(response);
        }

        [HttpGet("CalculateBuyBackPrice/{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> CalculateBuyBackPrice(string productId)
        {
            try
            {
                var price = await _productService.CalculateBuyBackPriceForSingleProductAsync(productId);
                return Ok(price);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("pay")]
        [Authorize(Roles = "Cashier")]
        public async Task<IActionResult> PayForBuyBackOutOfStore([FromBody] PaidOrderBuyBackReq request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _orderBuyBackService.PayForBuyBackAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("cancel")]
        [Authorize(Roles = "Cashier, Seller")]
        public async Task<IActionResult> CancelledOrderBuyBack(int orderBuyBackId)
        {
            try
            {
                var cancelledOrderBb = await _orderBuyBackService.CancelOrderBuyBackAsync(orderBuyBackId);
                if (cancelledOrderBb.OrderBuyBackId == null)
                {
                    return BadRequest("OrderBuyBack not found");
                }

                return Ok(cancelledOrderBb);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{orderBuyBackId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderBuyBackById(int orderBuyBackId)
        {
            try
            {
                var response = await _orderBuyBackService.GetOrderBuyBackById(orderBuyBackId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("invoice/pdf/{orderBuyBackId}")]
        public async Task<IActionResult> GetOrderBuyBackInvoicePdf(int orderBuyBackId)
        {
            try
            {
                var pdfBytes = await _invoiceService.GenerateOrderBuyBackInvoicePdfAsync(orderBuyBackId);
                return File(pdfBytes, "application/pdf", $"OrderBuyBackInvoice_{orderBuyBackId}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
