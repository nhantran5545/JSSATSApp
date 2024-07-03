using JSSATSAPI.BussinessObjects.RequestModels.CategoryRequest;
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
        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryTypeResponse> CreateCategoryTypeAsync(CreateCategoryTypeRequest request);
        Task<List<CategoryResponse>> GetAllCategoriesAsync();
        Task<List<CategoryTypeResponse>> GetAllCategoryTypesWithCategoriesAsync();
        Task<CategoryResponse> GetCategoryById(int category);
        Task<CategoryTypeResponse> GetCategoryTypeById(int category);
        Task<bool> UpdateCategoryAsync(int categoryId, CreateCategoryRequest request);
        Task<bool> UpdateCategoryTypeAsync(int categoryTypeId, CreateCategoryTypeRequest request);
    }
}
