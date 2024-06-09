using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest
{
    public class OrderBuyBackInStoreRequest
    {
        public string CustomerId { get; set; }
        public List<string> ProductIds { get; set; }
    }
}
