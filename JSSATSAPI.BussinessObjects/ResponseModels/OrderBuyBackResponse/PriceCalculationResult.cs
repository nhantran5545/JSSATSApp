using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackResponse
{
    public class PriceCalculationResult
    {
        public bool Success { get; set; }
        public decimal Price { get; set; }
        public string ErrorMessage { get; set; }
    }
}
