using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.RequestModels.CategoryRequest
{
    public class CreateCategoryRequest
    {
        public string CategoryName { get; set; } = null!;
        public decimal? DiscountRate { get; set; }
        public int CategoryTypeId { get; set; }
    }
}
