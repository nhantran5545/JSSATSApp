using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class OrderSellDetail
    {
        public OrderSellDetail()
        {
            WarrantyTickets = new HashSet<WarrantyTicket>();
        }

        public int OrderSellDetailId { get; set; }
        public int? OrderSellId { get; set; }
        public string ProductId { get; set; } = null!;
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }

        public virtual OrderSell? OrderSell { get; set; }
        public virtual Product Product { get; set; } = null!;
        public virtual ICollection<WarrantyTicket> WarrantyTickets { get; set; }
    }
}
