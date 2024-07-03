using JSSATSAPI.BussinessObjects.ResponseModels.DashboardResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IDashboardService
    {
        Task<DashboardCountsResponse> GetDashboardCountsAsync();
        Task<DashboardResponse> GetDashboardDataAsync();
        Task<SalesDashboardResponse> GetSalesDataAsync();
    }

}
