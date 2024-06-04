using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class OrderBuyBack
    {
        public OrderBuyBack()
        {
            OrderBuyBackDetails = new HashSet<OrderBuyBackDetail>();
        }

        public int OrderBuyBackId { get; set; }
        public int? CustomerId { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? FinalAmount { get; set; }
        public string? Status { get; set; }
        public DateTime? DateBuyBack { get; set; }

        public virtual ICollection<OrderBuyBackDetail> OrderBuyBackDetails { get; set; }
    }
}
