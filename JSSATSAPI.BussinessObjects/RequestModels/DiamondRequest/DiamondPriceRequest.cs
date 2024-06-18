using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.DiamondRequest
{
    public class DiamondPriceRequest
    {
        public string? Origin { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Carat weight must be greater than zero")]
        public decimal? CaratWeightFrom { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Carat weight must be greater than zero")]
        public decimal? CaratWeightTo { get; set; }

        [Required(ErrorMessage = "Color is required")]
        [RegularExpression("^(D|E|F|J)$", ErrorMessage = "Color must be one of the following values: D, E, F, J")]
        public string? Color { get; set; }

        [Required(ErrorMessage = "Clarity is required")]
        [RegularExpression("^(IF|VVS1|VVS2|VS1|VS2)$", ErrorMessage = "Clarity must be one of the following values: IF, VVS1, VVS2, VS1, VS2")]
        public string? Clarity { get; set; }

        [Required(ErrorMessage = "Cut is required")]
        [RegularExpression("^(Good|Excellent|Very Good)$", ErrorMessage = "Cut must be one of the following values: Good, Excellent, Very Good")]
        public string? Cut { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Sell price must be non-negative")]
        public decimal? SellPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Buy price must be non-negative")]
        public decimal? BuyPrice { get; set; }

        public DateTime? EffDate { get; set; }
    }
}
