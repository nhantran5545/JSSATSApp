using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.CustomerReqModels
{
    public class CustomerRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^(?:\+?(?:84|0))(?:\d{9,10})$", ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }
    }
}
