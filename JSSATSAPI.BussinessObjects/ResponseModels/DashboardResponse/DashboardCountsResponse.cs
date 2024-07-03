using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.DashboardResponse
{
    public class DashboardCountsResponse
    {
        public int OrderSellCount { get; set; }
        public int OrderBuyBackCount { get; set; }
        public int CustomerCount { get; set; }
    }
}
