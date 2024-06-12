using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public int? OrderSellId { get; set; }
        public int? PaymentTypeId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? OrderBuyBackId { get; set; }

        public virtual OrderBuyBack? OrderBuyBack { get; set; }
        public virtual OrderSell? OrderSell { get; set; }
        public virtual PaymentType? PaymentType { get; set; }
    }
}
