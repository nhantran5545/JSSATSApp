using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class MaterialPrice
    {
        public int MaterialPriceId { get; set; }
        public int MaterialId { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
        public DateTime EffDate { get; set; }

        public virtual Material Material { get; set; } = null!;
    }
}
