using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.WarrantyTicketResponse
{
    public class WarrantyTicketResponse
    {
        public int WarrantyId { get; set; }
        public int OrderSellDetailId { get; set; }
        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime? WarrantyStartDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
    }
}
