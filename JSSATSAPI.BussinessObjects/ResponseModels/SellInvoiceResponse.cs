﻿using JSSATSAPI.BussinessObjects.RequestModels.PaymentReq;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels
{
    public class SellInvoiceDTO
    {
        public int OrderSellId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int SellerId { get; set; }
        public string SellerFirstName { get; set; }
        public string SellerLastName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal InvidualPromotionDiscount { get; set; }
        public decimal MemberShipDiscount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public List<OrderSellDetailResponse> OrderSellDetails { get; set; } = new();
        public List<PaymentResponse> Payments { get; set; } = new();
    }
}
