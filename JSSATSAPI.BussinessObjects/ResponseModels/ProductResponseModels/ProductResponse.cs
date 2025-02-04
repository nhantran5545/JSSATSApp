﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels
{
    public class ProductResponse
    {
        public string ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Size { get; set; }
        public string? Img { get; set; }
        public int? CounterId { get; set; }
        public string CounterName { get; set; }
        public int? CategoryId { get; set; }
        public int? MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public decimal? MaterialWeight { get; set; }
        public string? DiamondCode { get; set; }
        public string? DiamondName { get; set; }
        public string CategoryName { get; set; }
        public decimal? ProductMaterialCost { get; set; }
        public decimal? ProductMaterialCostBuyBack { get; set; }
        public decimal? ProductDiamondCost { get; set; }
        public decimal? ProductDiamondCostBuyBack { get; set; }
        public decimal? MaterialCost { get; set; }
        public decimal? DiamondCost { get; set; }
        public decimal? ProductionCost { get; set; }
        public decimal? ProductPrice { get; set; }
        public decimal? BuyBackPrice { get; set; }
        public decimal? DiscountRate { get; set; }
        public decimal? PriceRate { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
    }
}
