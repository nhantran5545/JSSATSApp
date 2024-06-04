using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Account
    {
        public Account()
        {
            Counters = new HashSet<Counter>();
            OrderSells = new HashSet<OrderSell>();
        }

        public int AccountId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; }
        public decimal? Revenue { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<Counter> Counters { get; set; }
        public virtual ICollection<OrderSell> OrderSells { get; set; }
    }
}
