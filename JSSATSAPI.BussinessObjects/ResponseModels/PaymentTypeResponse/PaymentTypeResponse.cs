using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse
{
    public class PaymentTypeResponse
    {
        public int PaymentTypeId { get; set; }
        public string? PaymentTypeName { get; set; }
        public string? Status { get; set; }
    }
}
