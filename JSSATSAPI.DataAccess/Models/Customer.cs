using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Customer
    {
        public Customer()
        {
            OrderBuyBacks = new HashSet<OrderBuyBack>();
            OrderSells = new HashSet<OrderSell>();
        }

        public string CustomerId { get; set; } = null!;
        public int? TierId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? LoyaltyPoints { get; set; }

        public virtual Membership? Tier { get; set; }
        public virtual ICollection<OrderBuyBack> OrderBuyBacks { get; set; }
        public virtual ICollection<OrderSell> OrderSells { get; set; }
    }
}
