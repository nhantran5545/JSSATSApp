using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackResponse;
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

        public OrderBuyBacksController(IOrderBuyBackService orderBuyBackService)
        {
            _orderBuyBackService = orderBuyBackService;
        }

        [HttpPost("BuyBackProductOutOfStore")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> BuyBackProductOutOfStore([FromBody] OrderBuyBackRequest request)
        {
            if (request == null || request.OrderBuyBackDetails == null || !request.OrderBuyBackDetails.Any())
            {
                return BadRequest("Invalid request.");
            }

            var response = await _orderBuyBackService.BuyBackProductOutOfStoreAsync(request);
            return Ok(response);
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
    }
}
