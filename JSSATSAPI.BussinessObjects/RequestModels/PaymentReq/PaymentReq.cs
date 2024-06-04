using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.PaymentReq
{
    public class PaymentReq
    {
        public int PaymentTypeId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
