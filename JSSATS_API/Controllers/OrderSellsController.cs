using AutoMapper;
using Azure.Core;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.OrderSellReq;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse;
using JSSATSAPI.BussinessObjects.Service;
using JSSATSAPI.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderSellsController : ControllerBase
    {
        private readonly IOrderSellService _orderSellService;
        private readonly IInvoiceService _invoiceService;

        public OrderSellsController(IOrderSellService orderSellService, IInvoiceService invoiceService)
        {
            _orderSellService = orderSellService;
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Cashier")]
        public async Task<IActionResult> GetAllOrderSells()
        {
            try
            {
                var orderSells = await _orderSellService.GetOrderSells();
                return Ok(orderSells);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{orderSellsId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderSellById(string orderSellsId)
        {
            try
            {
                var orderSells = await _orderSellService.GetOrderSellById(orderSellsId);
                if (orderSells == null)
                {
                    return NotFound();
                }
                return Ok(orderSells);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "Seller")]
        public IActionResult CreateSellOrder([FromBody] OrderSellRequest request)
        {
            try
            {
                var orderSellResponse = _orderSellService.CreateSellOrder(request);
                return Ok(orderSellResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetOrdersSellByCustomer/{customerId}")]
        [Authorize]
        public ActionResult<IEnumerable<OrderSellResponse>> GetOrdersByCustomer(string customerId)
        {
            try
            {
                var response = _orderSellService.GetOrdersByCustomerId(customerId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("complete")]
        [Authorize(Roles = "Cashier")]
        public async Task<IActionResult> CompleteOrderSell(CompletedOrderSellResponse completedOrderSellDto)
        {
            try
            {
                var completedOrderSell = await _orderSellService.CompleteOrderSellAsync(completedOrderSellDto);
                if (completedOrderSell.OrderSellId == null)
                {
                    return BadRequest("OrderSell not found");
                }
                return Ok(completedOrderSell);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("cancel")]
        [Authorize(Roles = "Cashier")]
        public async Task<IActionResult> CancelOrderSell([FromBody] int orderSellId)
        {
            try
            {
                var cancelledOrderSell = await _orderSellService.CancelOrderSellAsync(orderSellId);
                return Ok(cancelledOrderSell);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("export/{orderSellId}")]
        [Authorize(Roles = "Cashier")]
        public async Task<IActionResult> ExportSellInvoice(int orderSellId)
        {
            try
            {
                var pdfData = await _invoiceService.ExportSellInvoiceAsync(orderSellId);
                return File(pdfData, "application/pdf", $"Invoice_OrderSell_{orderSellId}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("view/{orderSellId}")]
        [Authorize(Roles = "Cashier")]
        public async Task<IActionResult> ViewSellInvoice(int orderSellId)
        {
            try
            {
                var htmlContent = await _invoiceService.GetSellInvoiceHtmlAsync(orderSellId);
                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
