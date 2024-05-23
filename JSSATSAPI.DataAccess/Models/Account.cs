using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Account
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }
        public int? CounterId { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }

        public virtual Counter? Counter { get; set; }
    }
}
