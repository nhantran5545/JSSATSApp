using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest
{
    public class ReviewMaterialPriceRequest
    {
        public int MaterialId { get; set; }
        public decimal Weight { get; set; }
    }

}
