using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse
{
    public class DiamondResponse
    {
        public string DiamondCode { get; set; } = null!;
        public string? DiamondName { get; set; }
        public string Origin { get; set; } = null!;
        public decimal CaratWeightFrom { get; set; }
        public decimal CaratWeightTo { get; set; }
        public string Color { get; set; } = null!;
        public string Clarity { get; set; } = null!;
        public string Cut { get; set; } = null!;
        public decimal? Proportions { get; set; }
        public string? Polish { get; set; }
        public string? Symmetry { get; set; }
        public bool? Fluorescence { get; set; }
        public string? Status { get; set; }
    }
}
