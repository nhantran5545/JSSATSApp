using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.OrderSellReq
{
    public class OrderSellRequest
    {
        public string CustomerId { get; set; }
        public decimal? InvidualPromotionDiscount { get; set; }
        public string? PromotionReason { get; set; }
        public List<OrderSellDetailRequest> OrderSellDetails { get; set; }
    }
}
