using JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackDetailRes
{
    public class OrderBuyBackDetailInStoreResponse
    {
        public int OrderBuyBackDetailId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }
}
