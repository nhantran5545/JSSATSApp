using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.CategoryResponse
{
    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public int CategoryTypeId { get; set; }
        public string CategoryTypeName { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal? DiscountRate { get; set; }
        public string? Status { get; set; }
    }
}
