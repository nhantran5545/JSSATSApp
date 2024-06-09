using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTypesController : ControllerBase
    {
        private readonly IPaymentTypeService _paymentTypeService;

        public PaymentTypesController(IPaymentTypeService paymentTypeService)
        {
            _paymentTypeService = paymentTypeService;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PaymentTypeResponse>>> GetAllPaymentTypes()
        {
            try
            {
                var payment = await _paymentTypeService.GetAllPaymentTypeAsync();
                if (payment == null)
                {
                    return NotFound("there are no Issue Type");
                }

                return Ok(payment);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, "You are not authorized to access this resource.");
            }
        }
    }
}
