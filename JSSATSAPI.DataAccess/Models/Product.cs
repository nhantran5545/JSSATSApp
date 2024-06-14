using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderBuyBackDetails = new HashSet<OrderBuyBackDetail>();
            OrderSellDetails = new HashSet<OrderSellDetail>();
        }

        public string ProductId { get; set; } = null!;
        public string? ProductName { get; set; }
        public string? Size { get; set; }
        public string? Img { get; set; }
        public int? CounterId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MaterialCost { get; set; }
        public decimal? DiamondCost { get; set; }
        public decimal? ProductionCost { get; set; }
        public decimal? ProductPrice { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
        public decimal? PriceRate { get; set; }
        public decimal? BuyBackPrice { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Counter? Counter { get; set; }
        public virtual ICollection<OrderBuyBackDetail> OrderBuyBackDetails { get; set; }
        public virtual ICollection<OrderSellDetail> OrderSellDetails { get; set; }
    }
}
