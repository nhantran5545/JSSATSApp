using JSSATSAPI.BussinessObjects.RequestModels.PaymentReq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse
{
    public class CompletedOrderSellResponse
    {
        public int OrderSellId { get; set; }
        public List<PaymentReq> Payments { get; set; } = new();
    }
}
