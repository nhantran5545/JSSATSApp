using JSSATSAPI.BussinessObjects.RequestModels.PaymentReq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest
{
    public class PaidOrderBuyBackReq
    {
        public int OrderBuyBackId { get; set; }
        public List<PaymentRequest> Payments { get; set; } = new();
    }
}
