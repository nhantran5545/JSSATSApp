using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.MaterialResponse
{
    public class MaterialTypeResponse
    {
        public int MaterialTypeId { get; set; }
        public string? MaterialTypeName { get; set; }
        public ICollection<MaterialResponse> Materials { get; set; } = new List<MaterialResponse>();
    }
}
