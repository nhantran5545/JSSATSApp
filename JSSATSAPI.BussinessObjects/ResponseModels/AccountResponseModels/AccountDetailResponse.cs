using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.AccountResponseModels
{
    public class AccountDetailResponse
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
    }
}
