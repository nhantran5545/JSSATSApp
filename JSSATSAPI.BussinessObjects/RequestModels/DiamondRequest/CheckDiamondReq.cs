using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.DiamondRequest
{
    public class CheckDiamondReq
    {
        public string Origin { get; set; }
        public decimal CaratWeightFrom { get; set; }
        public decimal CaratWeightTo { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Cut { get; set; }
    }
}
