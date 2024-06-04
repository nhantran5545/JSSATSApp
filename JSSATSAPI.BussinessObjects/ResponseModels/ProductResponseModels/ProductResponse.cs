using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels
{
    public class ProductResponse
    {
        public string ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Size { get; set; }
        public string? Img { get; set; }
        public int? CounterId { get; set; }
        public string CounterName { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal? MaterialCost { get; set; }
        public decimal? DiamondCost { get; set; }
        public decimal? ProductionCost { get; set; }
        public decimal? ProductPrice { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
    }
}
