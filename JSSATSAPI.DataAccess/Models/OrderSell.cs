using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class OrderSell
    {
        public OrderSell()
        {
            OrderSellDetails = new HashSet<OrderSellDetail>();
            Payments = new HashSet<Payment>();
        }

        public int OrderSellId { get; set; }
        public string? CustomerId { get; set; }
        public int? SellerId { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? PromotionDiscount { get; set; }
        public decimal? MemberShipDiscount { get; set; }
        public decimal? FinalAmount { get; set; }
        public decimal? DiscountPercentForCustomer { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Status { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Account? Seller { get; set; }
        public virtual ICollection<OrderSellDetail> OrderSellDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
