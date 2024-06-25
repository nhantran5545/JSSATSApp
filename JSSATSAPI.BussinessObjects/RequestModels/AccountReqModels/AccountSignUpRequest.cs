using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.AccountReqModels
{
    public class AccountSignUpRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        public string? Phone { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }
        public int? CounterId { get; set; }
        public string? Role { get; set; }
    }

}
