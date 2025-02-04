﻿using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Counter
    {
        public Counter()
        {
            Products = new HashSet<Product>();
        }

        public int CounterId { get; set; }
        public string CounterName { get; set; } = null!;
        public int AccountId { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; }
    }
}
