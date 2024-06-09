using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class OrderBuyBackDetail
    {
        public int OrderBuyBackDetailId { get; set; }
        public int OrderBuyBackId { get; set; }
        public string? ProductId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Quantity { get; set; }
        public int? MaterialId { get; set; }
        public decimal? Weight { get; set; }
        public decimal? BuyBackProductName { get; set; }
        public string? Origin { get; set; }
        public decimal? CaratWeight { get; set; }
        public string? Color { get; set; }
        public string? Clarity { get; set; }
        public string? Cut { get; set; }
        public decimal? Price { get; set; }

        public virtual Material? Material { get; set; }
        public virtual OrderBuyBack OrderBuyBack { get; set; } = null!;
        public virtual Product? Product { get; set; }
    }
}
