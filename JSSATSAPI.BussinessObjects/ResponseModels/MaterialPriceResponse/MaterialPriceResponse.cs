using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.MaterialPriceResponse
{
    public class MaterialPriceResponse
    {
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
        public DateTime EffDate { get; set; }
    }
}
