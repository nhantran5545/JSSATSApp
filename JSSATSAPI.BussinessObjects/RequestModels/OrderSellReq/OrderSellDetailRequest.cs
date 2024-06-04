using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.OrderSellReq
{
    public class OrderSellDetailRequest
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }

}
