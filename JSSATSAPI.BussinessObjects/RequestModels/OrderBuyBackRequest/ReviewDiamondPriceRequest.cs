using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest
{
    public class ReviewDiamondPriceRequest
    {
        public string Origin { get; set; }
        public decimal CaratWeight { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Cut { get; set; }
    }

}
