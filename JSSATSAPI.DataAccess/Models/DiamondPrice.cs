using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class DiamondPrice
    {
        public int DiamondPriceId { get; set; }
        public string? Origin { get; set; }
        public decimal? CaratWeight { get; set; }
        public string? Color { get; set; }
        public string? Clarity { get; set; }
        public string? Cut { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? BuyPrice { get; set; }
        public DateTime? EffDate { get; set; }
    }
}
