using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.ProductReq
{
    public class AddProductRequest
    {
        public string ProductName { get; set; }
        public string Size { get; set; }
        public string Img { get; set; }
        public int? CounterId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MaterialCost { get; set; }
        public decimal? DiamondCost { get; set; }
        public decimal? ProductionCost { get; set; }
        public decimal? PriceRate { get; set; }
        public List<ProductMaterialRequest> Materials { get; set; } = new List<ProductMaterialRequest>();
        public List<ProductDiamondRequest> Diamonds { get; set; } = new List<ProductDiamondRequest>();
    }
}
