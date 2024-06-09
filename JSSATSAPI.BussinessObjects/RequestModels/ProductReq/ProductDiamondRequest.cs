using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.ProductReq
{
    [Keyless]
    public class ProductDiamondRequest
    {
        public string? DiamondCode { get; set; }
    }
}
