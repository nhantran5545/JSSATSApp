using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackDetailRes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackResponse
{
    public class CalculatePricesResponse
    {
        public decimal TotalAmount { get; set; }
        public List<OrderBuyBackDetailResponse> OrderBuyBackDetails { get; set; }
        public List<string> Errors { get; set; }
    }
}
