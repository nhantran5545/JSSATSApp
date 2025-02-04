﻿using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class CategoryType
    {
        public CategoryType()
        {
            Categories = new HashSet<Category>();
        }

        public int CategoryTypeId { get; set; }
        public string? CategoryTypeName { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}
