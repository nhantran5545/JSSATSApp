using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class PaymentType
    {
        public PaymentType()
        {
            Payments = new HashSet<Payment>();
        }

        public int PaymentTypeId { get; set; }
        public string? PaymentTypeName { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
