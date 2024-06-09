using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.ProductReq
{
    [Keyless]
    public class ProductMaterialRequest
    {
        public int MaterialId { get; set; }
        public decimal? Weight { get; set; }
    }
}
