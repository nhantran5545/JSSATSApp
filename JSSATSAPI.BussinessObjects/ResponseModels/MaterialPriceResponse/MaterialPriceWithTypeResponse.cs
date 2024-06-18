using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.MaterialPriceResponse
{
    public class MaterialPriceWithTypeResponse
    {
        public int MaterialTypeId { get; set; }
        public string? MaterialTypeName { get; set; }
        public List<Material1Response> Materials { get; set; } = new();
    }
}
