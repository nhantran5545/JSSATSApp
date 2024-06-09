using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest
{
    public class OrderBuyBackRequest
    {
        public string? CustomerId { get; set; }
        public List<OrderBuyBackDetailRequest> OrderBuyBackDetails { get; set; }
    }
}
