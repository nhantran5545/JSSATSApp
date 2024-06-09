using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.MaterialPriceResponse
{
    public class Material1Response
    {
        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public List<MaterialPriceResponse> MaterialPrices { get; set; }
    }
}
