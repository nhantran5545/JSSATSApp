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
    }
}
