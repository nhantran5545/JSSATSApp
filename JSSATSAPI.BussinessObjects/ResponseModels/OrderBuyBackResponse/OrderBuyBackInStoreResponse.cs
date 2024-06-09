using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackDetailRes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackResponse
{
    public class OrderBuyBackInStoreResponse
    {
        public int OrderBuyBackId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime DateBuyBack { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderBuyBackDetailInStoreResponse> OrderBuyBackDetails { get; set; }
    }
}
