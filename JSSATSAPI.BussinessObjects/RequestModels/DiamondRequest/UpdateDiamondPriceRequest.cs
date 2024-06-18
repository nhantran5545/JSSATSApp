using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.DiamondRequest
{
    public class UpdateDiamondPriceRequest
    {
        [Range(0, double.MaxValue, ErrorMessage = "Sell price must be non-negative")]
        public decimal? SellPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Buy price must be non-negative")]
        public decimal? BuyPrice { get; set; }
    }
}
