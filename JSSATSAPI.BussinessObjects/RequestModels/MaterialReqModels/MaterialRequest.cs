using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.MaterialReqModels
{
    public class MaterialRequest
    {
        public string? MaterialName { get; set; }
        public int? MaterialTypeId { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
    }

}
