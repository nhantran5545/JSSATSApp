using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.DashboardResponse
{
    public class DashboardResponse
    {
        public decimal TotalRevenue { get; set; }
        public List<CategoryRevenue> RevenueByCategory { get; set; }
        public List<OrderSummary> Orders { get; set; }
    }
}
