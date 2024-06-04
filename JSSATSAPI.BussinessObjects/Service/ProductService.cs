using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IDiamondRepository  _diamondRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper , IDiamondRepository diamondRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _diamondRepository = diamondRepository;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productResponseModels = new List<ProductResponse>();

            foreach (var product in products)
            {
                var productMaterials = _productRepository.GetProductMaterialsByProductId(product.ProductId); 

                decimal productionCost = product.ProductionCost ?? 0;
                decimal diamondCost = product.DiamondCost ?? 0;
                decimal materialCost = product.MaterialCost ?? 0; 

                decimal totalMaterialCost = 0;
                foreach (var productMaterial in productMaterials)
                {
                    var materialPrice = _productRepository.GetMaterialPriceById(productMaterial.MaterialId);
                    if (materialPrice != null)
                    {
                        totalMaterialCost += materialPrice.SellPrice * (productMaterial.Weight ?? 0);
                    }
                }

                // giá vốn
                decimal costPrice = (totalMaterialCost + productionCost + diamondCost + materialCost);

                // Tỉ lệ áp giá 
                decimal priceRatePercent = product.PriceRate ?? 0; 
                decimal priceRateDecimal = priceRatePercent / 100; 

                // Tính giá bán
                decimal productPrice = costPrice * (1 + priceRateDecimal); 

                var productResponseModel = _mapper.Map<ProductResponse>(product);
                productResponseModel.ProductPrice = productPrice;

                productResponseModels.Add(productResponseModel);
            }

            return productResponseModels;
        }
        public IEnumerable<ProductResponse> GetProductsAvaiable()
        {
            var products = _productRepository.GetProductsByStatus();
            var productResponseModels = new List<ProductResponse>();

            foreach (var product in products)
            {
                var productMaterials = _productRepository.GetProductMaterialsByProductId(product.ProductId);

                decimal productionCost = product.ProductionCost ?? 0;
                decimal diamondCost = product.DiamondCost ?? 0;
                decimal materialCost = product.MaterialCost ?? 0;

                decimal totalMaterialCost = 0;
                foreach (var productMaterial in productMaterials)
                {
                    var materialPrice = _productRepository.GetMaterialPriceById(productMaterial.MaterialId);
                    if (materialPrice != null)
                    {
                        totalMaterialCost += materialPrice.SellPrice * (productMaterial.Weight ?? 0);
                    }
                }

                // Tính  giá vốn
                decimal costPrice = (totalMaterialCost + productionCost + diamondCost + materialCost);

                // Tỉ lệ áp giá 
                decimal priceRatePercent = product.PriceRate ?? 0;
                decimal priceRateDecimal = priceRatePercent / 100;

                // Tính giá bán
                decimal productPrice = costPrice * (1 + priceRateDecimal);

                // Cập nhật giá sản phẩm vào database
                product.ProductPrice = productPrice;
                _productRepository.UpdateProductPrice(product.ProductId, productPrice);

                var productResponseModel = _mapper.Map<ProductResponse>(product);
                productResponseModel.ProductPrice = productPrice;

                productResponseModels.Add(productResponseModel);
            }

            return productResponseModels;
        }

    }
}
