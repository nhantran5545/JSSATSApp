using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Material
    {
        public Material()
        {
            MaterialPrices = new HashSet<MaterialPrice>();
            OrderBuyBackDetails = new HashSet<OrderBuyBackDetail>();
        }

        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public int? MaterialTypeId { get; set; }

        public virtual MaterialType? MaterialType { get; set; }
        public virtual ICollection<MaterialPrice> MaterialPrices { get; set; }
        public virtual ICollection<OrderBuyBackDetail> OrderBuyBackDetails { get; set; }
    }
}
