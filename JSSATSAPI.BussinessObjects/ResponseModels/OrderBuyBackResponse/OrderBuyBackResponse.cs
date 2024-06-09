using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackDetailRes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackResponse
{
    public class OrderBuyBackResponse
    {
        public int OrderBuyBackId { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? DateBuyBack { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? FinalAmount { get; set; }
        public string? Status { get; set; }
        public List<OrderBuyBackDetailResponse> OrderBuyBackDetails { get; set; } = new List<OrderBuyBackDetailResponse>();

        public List<string> Errors { get; set; } = new List<string>();
    }
}
