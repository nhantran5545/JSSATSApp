using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class ProductDiamond
    {
        public string? ProductId { get; set; }
        public string? DiamondCode { get; set; }

        public virtual Diamond? DiamondCodeNavigation { get; set; }
        public virtual Product? Product { get; set; }
    }
}
