using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.CustomerReqModels;
using JSSATSAPI.BussinessObjects.RequestModels.ProductReq;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.BussinessObjects.Service;
using JSSATSAPI.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpPost("create")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> CreateCustomers([FromBody] CustomerRequest request)
        {
            if (request == null)
            {
                return BadRequest("CustomerRequest is null.");
            }

            try
            {
                var response = await _customerService.CreateCustomerAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Message: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomers();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetCustomerById(string customerId)
        {
            try
            {
                var customer = await _customerService.GetCustomerById(customerId);
                if (customer == null)
                {
                    return NotFound();
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
