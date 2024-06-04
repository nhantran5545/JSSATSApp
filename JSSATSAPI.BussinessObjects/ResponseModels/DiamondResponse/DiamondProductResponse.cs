﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse
{
    public class DiamondProductResponse
    {
        public string DiamondCode { get; set; }
        public string DiamondName { get; set; }
        public string Origin { get; set; }
        public decimal CaratWeight { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Cut { get; set; }
        public decimal Proportions { get; set; }
        public string Polish { get; set; }
        public string Symmetry { get; set; }
        public bool Fluorescence { get; set; }
        public string Status { get; set; }
        public decimal SellPrice { get; set; }
    }

}
