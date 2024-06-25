using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse
{
    public class DiamondWithPriceResponse
    {
        public string DiamondCode { get; set; }
        public string? DiamondName { get; set; }
        public string Origin { get; set; }
        public decimal CaratWeightFrom { get; set; }
        public decimal CaratWeightTo { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Cut { get; set; }
        public decimal? Proportions { get; set; }
        public string? Polish { get; set; }
        public string? Symmetry { get; set; }
        public bool? Fluorescence { get; set; }
        public string? Status { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? BuyPrice { get; set; }
        public DateTime? EffDate { get; set; }
    }
}
