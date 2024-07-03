using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.CategoryRequest;
using JSSATSAPI.BussinessObjects.RequestModels.CounterRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.CategoryResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.CounterResponse;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using JSSATSAPI.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public async Task<CategoryTypeResponse> GetCategoryTypeById(int category)
        {
            var categoryID = await _categoryTypeRepository.GetByIdAsync(category);
            return _mapper.Map<CategoryTypeResponse>(categoryID);
        }

        public async Task<CategoryResponse> GetCategoryById(int category)
        {
            var categoryID = await _categoryRepository.GetByIdAsync(category);
            return _mapper.Map<CategoryResponse>(categoryID);
        }
        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<List<CategoryResponse>>(categories);
            return categoryDtos;
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            if (!Validator.TryValidateObject(request, new ValidationContext(request), null, true))
            {
                throw new ValidationException("Request is invalid");
            }
            var categoryType = await _categoryTypeRepository.GetByIdAsync(request.CategoryTypeId);
            if (categoryType == null)
            {
                throw new Exception($"CategoryType with ID {request.CategoryTypeId} not found");
            }
            var newC = new Category
            {
                CategoryName = request.CategoryName,
                DiscountRate = request.DiscountRate,
                CategoryTypeId =  request.CategoryTypeId,
                Status = "Active",
            };
            await _categoryRepository.AddAsync(newC);
            _categoryRepository.SaveChanges();


            var customerResponse = _mapper.Map<CategoryResponse>(newC);

            return customerResponse;
        }



        public async Task<CategoryTypeResponse> CreateCategoryTypeAsync(CreateCategoryTypeRequest request)
        {
            if (!Validator.TryValidateObject(request, new ValidationContext(request), null, true))
            {
                throw new ValidationException("Request is invalid");
            }
            var newCT = new CategoryType
            {
                CategoryTypeName = request.CategoryTypeName,
                Status = "Active",
            };
            await _categoryTypeRepository.AddAsync(newCT);
            _categoryTypeRepository.SaveChanges();


            var customerResponse = _mapper.Map<CategoryTypeResponse>(newCT);

            return customerResponse;
        }

        public async Task<bool> UpdateCategoryTypeAsync(int categoryTypeId, CreateCategoryTypeRequest request)
        {
            var categoryType = await _categoryTypeRepository.GetByIdAsync(categoryTypeId);
            if (categoryType == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(request.CategoryTypeName))
            {
                categoryType.CategoryTypeName = request.CategoryTypeName;
            }

            _categoryTypeRepository.Update(categoryType);
            var result = _categoryTypeRepository.SaveChanges();
            if (result < 1)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, CreateCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return false;
            }

            category.CategoryName = request.CategoryName;
            category.DiscountRate = request.DiscountRate;
            category.CategoryTypeId = request.CategoryTypeId;

            _categoryRepository.Update(category);

            var result = await _categoryRepository.SaveChangesAsync();
            return result > 0;
        }

    }
}
