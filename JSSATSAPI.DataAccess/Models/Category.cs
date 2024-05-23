using System;
using System.Collections.Generic;

namespace JSSATSAPI.DataAccess.Models
{
    public partial class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int CategoryTypeId { get; set; }
        public string Status { get; set; } = null!;

        public virtual CategoryType CategoryType { get; set; } = null!;
    }
}
