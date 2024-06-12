using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class OrderBuyBack
    {
        public OrderBuyBack()
        {
            OrderBuyBackDetails = new HashSet<OrderBuyBackDetail>();
            Payments = new HashSet<Payment>();
        }

        public int OrderBuyBackId { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? DateBuyBack { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? FinalAmount { get; set; }
        public string? Status { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual ICollection<OrderBuyBackDetail> OrderBuyBackDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
