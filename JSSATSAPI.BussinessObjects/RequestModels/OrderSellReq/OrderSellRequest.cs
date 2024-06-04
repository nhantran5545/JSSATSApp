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
        public int SellerId { get; set; }
        public decimal? PromotionDiscount { get; set; }
        public decimal? MemberShipDiscount { get; set; }
        public decimal? DiscountPercentForCustomer { get; set; }
        public List<OrderSellDetailRequest> OrderSellDetails { get; set; }
    }
}
