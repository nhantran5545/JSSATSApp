using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.CategoryResponse;
using JSSATSAPI.DataAccess.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryTypeRepository _categoryTypeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryTypeRepository categoryTypeRepository, IMapper mapper , ICategoryRepository categoryRepository)
        {
            _categoryTypeRepository = categoryTypeRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryTypeResponse>> GetAllCategoryTypesWithCategoriesAsync()
        {
            var categoryTypes = await _categoryTypeRepository.GetAllAsync();
            var categoryTypeDtos = _mapper.Map<List<CategoryTypeResponse>>(categoryTypes);
            return categoryTypeDtos;
        }
        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<List<CategoryResponse>>(categories);
            return categoryDtos;
        }
    }
}
