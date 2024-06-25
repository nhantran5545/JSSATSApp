using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.WarrantyTicketResponse;
using JSSATSAPI.BussinessObjects.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyTicketsController : ControllerBase
    {
        private readonly IWarrantyTicketService _warrantyTicketService;

        public WarrantyTicketsController(IWarrantyTicketService warrantyTicketService)
        {
            _warrantyTicketService = warrantyTicketService;
        }

/*        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<WarrantyTicketResponse>>> GetAllWarrantyTickets()
        {
            try
            {
                var wt = await _warrantyTicketService.GetAllWarrantyTicketsAsync();
                if (wt == null)
                {
                    return NotFound("there are no Warranty Ticket");
                }

                return Ok(wt);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }*/
    }
}
