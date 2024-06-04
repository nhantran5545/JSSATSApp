using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Diamond
    {
        public string DiamondCode { get; set; } = null!;
        public string DiamondName { get; set; } = null!;
        public int DiamondPriceId { get; set; }
        public string Origin { get; set; } = null!;
        public decimal CaratWeight { get; set; }
        public string Color { get; set; } = null!;
        public string Clarity { get; set; } = null!;
        public string Cut { get; set; } = null!;
        public decimal Proportions { get; set; }
        public string? Polish { get; set; }
        public string? Symmetry { get; set; }
        public bool? Fluorescence { get; set; }
        public string? Status { get; set; }

        public virtual DiamondPrice DiamondPrice { get; set; } = null!;
    }
}
