using JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse
{
    public class CustomerResponse
    {
        public string CustomerId { get; set; }
        public int? TierId { get; set; }
        public string? TierName { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? LoyaltyPoints { get; set; }
        public int? DiscountPercent { get; set; }
    }

}
