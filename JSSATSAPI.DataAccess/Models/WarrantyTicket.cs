using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class WarrantyTicket
    {
        public int WarrantyId { get; set; }
        public int OrderSellDetailId { get; set; }
        public string ProductId { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime? WarrantyStartDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }

        public virtual OrderSellDetail OrderSellDetail { get; set; } = null!;
    }
}
