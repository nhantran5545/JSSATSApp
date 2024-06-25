using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.DashboardResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<DashboardResponse>> GetDashboardData()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return Ok(dashboardData);
        }

        [HttpGet("ProductSaleData")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<SalesDashboardResponse>> GetProductSalesData()
        {
            var salesData = await _dashboardService.GetSalesDataAsync();
            return Ok(salesData);
        }
    }
}
