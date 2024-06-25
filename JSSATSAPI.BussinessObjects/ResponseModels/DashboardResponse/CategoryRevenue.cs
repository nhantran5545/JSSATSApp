using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.DashboardResponse
{
    public class CategoryRevenue
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Revenue { get; set; }
    }
}
