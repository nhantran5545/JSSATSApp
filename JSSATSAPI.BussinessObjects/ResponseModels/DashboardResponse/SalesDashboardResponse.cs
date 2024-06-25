using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.DashboardResponse
{
    public class SalesDashboardResponse
    {
        public List<ProductSalesResponse> DailySales { get; set; } = new();
        public List<ProductSalesResponse> WeeklySales { get; set; } = new();
        public List<ProductSalesResponse> MonthlySales { get; set; } = new();
        public List<ProductSalesResponse> YearlySales { get; set; } = new();
    }
}
