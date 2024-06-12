using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.ResponseModels.CategoryResponse
{
    public class CategoryTypeResponse
    {
        public int CategoryTypeId { get; set; }
        public string? CategoryTypeName { get; set; }
        public string? Status { get; set; }
        public List<CategoryResponse> Categories { get; set; } = new List<CategoryResponse>();
    }
}
