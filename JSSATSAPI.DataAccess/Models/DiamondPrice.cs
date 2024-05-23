using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class DiamondPrice
    {
        public int DiamondPriceId { get; set; }
        public string Origin { get; set; } = null!;
        public decimal CaratWeight { get; set; }
        public string Color { get; set; } = null!;
        public string Clarity { get; set; } = null!;
        public string Cut { get; set; } = null!;
        public decimal SellPrice { get; set; }
        public decimal BuyPrice { get; set; }
        public DateTime EffDate { get; set; }
    }
}
