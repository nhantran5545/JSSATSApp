﻿using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal? DiscountRate { get; set; }
        public int CategoryTypeId { get; set; }
        public string? Status { get; set; }

        public virtual CategoryType CategoryType { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; }
    }
}
