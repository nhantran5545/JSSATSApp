using AutoMapper;
using JSSATSAPI.BussinessObjects.InheritanceClass;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IProductDiamondRepository _productDiamondRepository;
        private readonly IProductMaterialRepository _productMaterialRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<ProductHub> _hubContext;

        public ProductService(IProductRepository productRepository, IDiamondRepository diamondRepository, 
            IDiamondPriceRepository diamondPriceRepository, IProductDiamondRepository productDiamondRepository, 
            IProductMaterialRepository productMaterialRepository, IMapper mapper, IHubContext<ProductHub> hubContext)
        {
            _productRepository = productRepository;
            _diamondRepository = diamondRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _productDiamondRepository = productDiamondRepository;
            _productMaterialRepository = productMaterialRepository;
            _mapper = mapper;
            _hubContext = hubContext;
        }


        //get All ProductMaterial
        public async Task<IEnumerable<ProductResponse>> GetAllProductMaterialsAsync()
        {
            var products = await _productMaterialRepository.GetAllProductMaterials();
            var productResponseModels = new List<ProductResponse>();

            foreach (var product in products)
            {
                var p = product.Product;
                var productMaterials = _productMaterialRepository.GetProductMaterialsByProductId(product.ProductId);

                decimal productionCost = p.ProductionCost ?? 0;
                decimal diamondCost = p.DiamondCost ?? 0;
                decimal materialCost = p.MaterialCost ?? 0;

                decimal totalMaterialCost = 0;
                foreach (var productMaterial in productMaterials)
                {
                    var materialPrice = _productMaterialRepository.GetMaterialPriceById(productMaterial.MaterialId);
                    if (materialPrice != null)
                    {
                        totalMaterialCost += materialPrice.SellPrice * (productMaterial.Weight ?? 0);
                    }
                }

                decimal costPrice = totalMaterialCost + productionCost + diamondCost + materialCost;
                decimal priceRatePercent = p.PriceRate ?? 0;
                decimal priceRateDecimal = priceRatePercent / 100;
                decimal productPrice = costPrice * (1+ priceRateDecimal);

                // Cập nhật giá sản phẩm vào database
                p.ProductPrice = productPrice;
                _productRepository.UpdateProductPrice(product.ProductId, productPrice);
                _productRepository.SaveChanges();

                var productResponseModel = _mapper.Map<ProductResponse>(p);
                productResponseModel.ProductPrice = productPrice;

                productResponseModels.Add(productResponseModel);
            }

            // Phát sóng cập nhật sản phẩm
            await _hubContext.Clients.All.SendAsync("ReceiveProducts", productResponseModels);

            return productResponseModels;
        }


        //get All ProductMaterial Avaiable
        public async Task<IEnumerable<ProductResponse>> GetAllProductMaterialsAvaiableAsync()
        {
            var products = await _productMaterialRepository.GetAllProductMaterialsAvaiable();
            var productResponseModels = new List<ProductResponse>();

            foreach (var product in products)
            {
                var p = product.Product;
                var productMaterials = _productMaterialRepository.GetProductMaterialsByProductId(product.ProductId);

                decimal productionCost = p.ProductionCost ?? 0;
                decimal diamondCost = p.DiamondCost ?? 0;
                decimal materialCost = p.MaterialCost ?? 0;

                decimal totalMaterialCost = 0;
                foreach (var productMaterial in productMaterials)
                {
                    var materialPrice = _productMaterialRepository.GetMaterialPriceById(productMaterial.MaterialId);
                    if (materialPrice != null)
                    {
                        totalMaterialCost += materialPrice.SellPrice * (productMaterial.Weight ?? 0);
                    }
                }

                decimal costPrice = totalMaterialCost + productionCost + diamondCost + materialCost;
                decimal priceRatePercent = p.PriceRate ?? 0;
                decimal priceRateDecimal = priceRatePercent / 100;
                decimal productPrice = costPrice * (1 + priceRateDecimal);

                // Cập nhật giá sản phẩm vào database
                p.ProductPrice = productPrice;
                _productRepository.UpdateProductPrice(product.ProductId, productPrice);
                _productRepository.SaveChanges();

                var productResponseModel = _mapper.Map<ProductResponse>(p);
                productResponseModel.ProductPrice = productPrice;

                productResponseModels.Add(productResponseModel);
            }

            // Phát sóng cập nhật sản phẩm
            await _hubContext.Clients.All.SendAsync("ReceiveProducts", productResponseModels);

            return productResponseModels;
        }


        //get All ProductDiamond
        public async Task<IEnumerable<ProductResponse>> GetAllProductDiamondsAsync()
        {
            var productDiamonds = await _productDiamondRepository.GetAllProductDiamondsAsync();
            var productResponse = new List<ProductResponse>();

            foreach (var productDiamond in productDiamonds)
            {
                var product = productDiamond.Product;
                var diamond = productDiamond.DiamondCodeNavigation;
                var latestPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(diamond.Origin, diamond.CaratWeight, diamond.Color, diamond.Clarity, diamond.Cut);

                if (product != null && latestPrice != null)
                {
                    decimal productCost = (latestPrice.SellPrice ?? 0) + (product.ProductionCost ?? 0) + (product.DiamondCost ?? 0) + (product.MaterialCost ?? 0);
                    // Tỉ lệ áp giá 
                    decimal priceRatePercent = product.PriceRate ?? 0;
                    decimal priceRateDecimal = priceRatePercent / 100;
                    decimal productPrice = productCost * (1 + priceRateDecimal);

                    // Cập nhật giá sản phẩm vào database
                    product.ProductPrice = productPrice;
                    _productRepository.UpdateProductPrice(product.ProductId, productPrice);
                    _productRepository.SaveChanges();

                    var productResponseModel = _mapper.Map<ProductResponse>(product);
                    productResponseModel.ProductPrice = productPrice;

                    productResponse.Add(productResponseModel);
                }
                // Phát sóng cập nhật sản phẩm
                await _hubContext.Clients.All.SendAsync("ReceiveProducts", productResponse);
            }
            return productResponse;
        }

        //get All ProductDiamond avaiable
        public async Task<IEnumerable<ProductResponse>> GetAllProductDiamondsAvaivaleAsync()
        {
            var productDiamonds = await _productDiamondRepository.GetAllProductDiamondsAvaiableAsync();
            var productResponse = new List<ProductResponse>();

            foreach (var productDiamond in productDiamonds)
            {
                var product = productDiamond.Product;
                var diamond = productDiamond.DiamondCodeNavigation;
                var latestPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(diamond.Origin, diamond.CaratWeight, diamond.Color, diamond.Clarity, diamond.Cut);

                if (product != null && latestPrice != null)
                {
                    decimal productCost = (latestPrice.SellPrice ?? 0) + (product.ProductionCost ?? 0) + (product.DiamondCost ?? 0) + (product.MaterialCost ?? 0);
                    // Tỉ lệ áp giá 
                    decimal priceRatePercent = product.PriceRate ?? 0;
                    decimal priceRateDecimal = priceRatePercent / 100;
                    decimal productPrice = productCost * (1 + priceRateDecimal);

                    // Cập nhật giá sản phẩm vào database
                    product.ProductPrice = productPrice;
                    _productRepository.UpdateProductPrice(product.ProductId, productPrice);
                    _productRepository.SaveChanges();

                    var productResponseModel = _mapper.Map<ProductResponse>(product);
                    productResponseModel.ProductPrice = productPrice;

                    productResponse.Add(productResponseModel);
                }
                // Phát sóng cập nhật sản phẩm
                await _hubContext.Clients.All.SendAsync("ReceiveProducts", productResponse);
            }
            return productResponse;
        }
    }
}
