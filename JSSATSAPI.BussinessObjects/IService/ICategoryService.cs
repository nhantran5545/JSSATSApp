using JSSATSAPI.BussinessObjects.ResponseModels.CategoryResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetAllCategoriesAsync();
        Task<List<CategoryTypeResponse>> GetAllCategoryTypesWithCategoriesAsync();
    }
}
