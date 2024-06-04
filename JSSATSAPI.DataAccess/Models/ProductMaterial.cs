using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class ProductMaterial
    {
        public int MaterialId { get; set; }
        public string? ProductId { get; set; }
        public decimal? Weight { get; set; }

        public virtual Material Material { get; set; } = null!;
        public virtual Product? Product { get; set; }
    }
}
