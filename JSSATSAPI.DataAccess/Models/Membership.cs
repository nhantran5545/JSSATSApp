using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Membership
    {
        public Membership()
        {
            Customers = new HashSet<Customer>();
        }

        public int TierId { get; set; }
        public string? TierName { get; set; }
        public int? DiscountPercent { get; set; }
        public int? PointThreshold { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
