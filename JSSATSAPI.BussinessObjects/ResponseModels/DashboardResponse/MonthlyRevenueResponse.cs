using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.DashboardResponse
{
    public class MonthlyRevenueResponse
    {
        public int AccountId { get; set; }
        public string FullName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
    }

}
