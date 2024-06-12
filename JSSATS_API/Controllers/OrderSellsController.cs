﻿using AutoMapper;
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
        private readonly IAccountService _accountService;

        public OrderSellsController(IOrderSellService orderSellService, IInvoiceService invoiceService , IAccountService accountService)
        {
            _orderSellService = orderSellService;
            _invoiceService = invoiceService;
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize]
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
        public async Task<IActionResult> GetOrderSellById(int orderSellsId)
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


        [HttpGet("orderSellBySeller")]
        [Authorize]
        public async Task<IActionResult> GetOrderSellPaidBySellerId()
        {
            try
            {
                var sellerId =  _accountService.GetAccountIdFromToken();
                var orderSells = await _orderSellService.GetOrderSellBySellerId(sellerId);
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

        [HttpGet("orderSellProcessingBySeller")]
        [Authorize]
        public async Task<IActionResult> GetOrderSellProcessingBySellerId()
        {
            try
            {
                var sellerId = _accountService.GetAccountIdFromToken();
                var orderSells = await _orderSellService.GetOrderSellProcessingBySellerId(sellerId);
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

        [HttpGet("orderSellDeliveredBySeller")]
        [Authorize]
        public async Task<IActionResult> GetOrderSellDeliveredBySellerId()
        {
            try
            {
                var sellerId = _accountService.GetAccountIdFromToken();
                var orderSells = await _orderSellService.GetOrderSellDeliveredBySellerId(sellerId);
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

        [HttpPut("update-discount/{orderSellId}")]
        [Authorize]
        public async Task<IActionResult> UpdateIndividualPromotionDiscountAsync(int orderSellId, [FromBody] decimal? newDiscount)
        {
            try
            {
                if (newDiscount.HasValue)
                {
                    await _orderSellService.UpdateIndividualPromotionDiscountAsync(orderSellId, newDiscount);
                }
                else
                {
                    return NoContent();
                }

                return NoContent();
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

        [HttpPost("paid")]
        [Authorize(Roles = "Cashier")]
        public async Task<IActionResult> PaidOrderSell(CompletedOrderSellResponse completedOrderSellDto)
        {
            try
            {
                var paidOrderSell = await _orderSellService.PaidOrderSellAsync(completedOrderSellDto);
                if (paidOrderSell.OrderSellId == null)
                {
                    return BadRequest("OrderSell not found");
                }
                return Ok(paidOrderSell);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("deliveried")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeliveriedOrderSell(int orderSellId)
        {
            try
            {
                var deliveredOrderSell = await _orderSellService.DeliveredOrderSellAsync(orderSellId);
                if (deliveredOrderSell.OrderSellId == null)
                {
                    return BadRequest("OrderSell not found");
                }
                return Ok(deliveredOrderSell);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("cancel")]
        [Authorize(Roles = "Cashier, Seller")]
        public async Task<IActionResult> CancelledOrderSell( int orderSellId)
        {
            try
            {
                var cancelledOrderSell = await _orderSellService.CancelOrderSellAsync(orderSellId);
                if (cancelledOrderSell.OrderSellId == null)
                {
                    return BadRequest("OrderSell not found");
                }

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
