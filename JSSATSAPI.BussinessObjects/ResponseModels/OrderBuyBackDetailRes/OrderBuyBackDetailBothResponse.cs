using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackDetailRes
{
    public class OrderBuyBackDetailBothResponse
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string? BuyBackProductName { get; set; }
        public int? Quantity { get; set; }
        public int? MaterialId { get; set; }
        public decimal? Weight { get; set; }
        public string? Origin { get; set; }
        public decimal? CaratWeight { get; set; }
        public string? Color { get; set; }
        public string? Clarity { get; set; }
        public string? Cut { get; set; }
        public decimal? Price { get; set; }
    }
}
