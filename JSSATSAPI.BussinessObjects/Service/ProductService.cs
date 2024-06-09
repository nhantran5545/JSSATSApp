using AutoMapper;
using iText.Commons.Actions.Contexts;
using JSSATSAPI.BussinessObjects.InheritanceClass;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.ProductReq;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using JSSATSAPI.DataAccess.Repository;
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

        public async Task<IEnumerable<ProductResponse>> GetAllProductMaterialsAndDiamondsAsync()
        {
            var products = await _productMaterialRepository.GetAllProductMaterials();
            var productDiamonds = await _productDiamondRepository.GetAllProductDiamondsAsync();
            var productResponseModels = new List<ProductResponse>();

            foreach (var product in products.Select(p => p.Product).Concat(productDiamonds.Select(pd => pd.Product)).Distinct())
            {
                decimal productionCost = product.ProductionCost ?? 0;
                decimal diamondCost = product.DiamondCost ?? 0;
                decimal materialCost = product.MaterialCost ?? 0;

                decimal totalMaterialCost = 0;
                var productMaterials = _productMaterialRepository.GetProductMaterialsByProductId(product.ProductId);
                foreach (var productMaterial in productMaterials)
                {
                    var materialPrice = _productMaterialRepository.GetMaterialPriceById(productMaterial.MaterialId.Value);
                    if (materialPrice != null)
                    {
                        totalMaterialCost += materialPrice.SellPrice * (productMaterial.Weight ?? 0);
                    }
                }

                decimal totalDiamondCost = 0;
                var productDiamondsList = productDiamonds.Where(pd => pd.ProductId == product.ProductId);
                foreach (var productDiamond in productDiamondsList)
                {
                    var diamond = productDiamond.DiamondCodeNavigation;
                    var latestPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(diamond.Origin, diamond.CaratWeight, diamond.Color, diamond.Clarity, diamond.Cut);

                    if (latestPrice != null)
                    {
                        totalDiamondCost += latestPrice.SellPrice ?? 0;
                    }
                }

                decimal costPrice = totalMaterialCost + productionCost + diamondCost + totalDiamondCost;
                decimal priceRatePercent = product.PriceRate ?? 0;
                decimal priceRateDecimal = priceRatePercent / 100;
                decimal productPrice = costPrice * (1 + priceRateDecimal);

                // Cập nhật giá sản phẩm vào database
                product.ProductPrice = productPrice;
                _productRepository.UpdateProductPrice(product.ProductId, productPrice);
                _productRepository.SaveChanges();

                var productResponseModel = _mapper.Map<ProductResponse>(product);
                productResponseModel.ProductPrice = productPrice;

                productResponseModels.Add(productResponseModel);
            }

            await _hubContext.Clients.All.SendAsync("ReceiveProducts", productResponseModels);

            return productResponseModels;
        }


        public async Task<IEnumerable<ProductResponse>> GetAllProductMaterialsAndDiamondsAvaiableAsync()
        {
            var products = await _productMaterialRepository.GetAllProductMaterialsAvaiable();
            var productDiamonds = await _productDiamondRepository.GetAllProductDiamondsAvaiableAsync();
            var productResponseModels = new List<ProductResponse>();

            foreach (var product in products.Select(p => p.Product).Concat(productDiamonds.Select(pd => pd.Product)).Distinct())
            {
                decimal productionCost = product.ProductionCost ?? 0;
                decimal diamondCost = product.DiamondCost ?? 0;
                decimal materialCost = product.MaterialCost ?? 0;

                decimal totalMaterialCost = 0;
                var productMaterials = _productMaterialRepository.GetProductMaterialsByProductId(product.ProductId);
                foreach (var productMaterial in productMaterials)
                {
                    var materialPrice = _productMaterialRepository.GetMaterialPriceById(productMaterial.MaterialId.Value);
                    if (materialPrice != null)
                    {
                        totalMaterialCost += materialPrice.SellPrice * (productMaterial.Weight ?? 0);
                    }
                }

                decimal totalDiamondCost = 0;
                var productDiamondsList = productDiamonds.Where(pd => pd.ProductId == product.ProductId);
                foreach (var productDiamond in productDiamondsList)
                {
                    var diamond = productDiamond.DiamondCodeNavigation;
                    var latestPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(diamond.Origin, diamond.CaratWeight, diamond.Color, diamond.Clarity, diamond.Cut);

                    if (latestPrice != null)
                    {
                        totalDiamondCost += latestPrice.SellPrice ?? 0;
                    }
                }

                decimal costPrice = totalMaterialCost + productionCost + diamondCost + totalDiamondCost;
                decimal priceRatePercent = product.PriceRate ?? 0;
                decimal priceRateDecimal = priceRatePercent / 100;
                decimal productPrice = costPrice * (1 + priceRateDecimal);

                // Cập nhật giá sản phẩm vào database
                product.ProductPrice = productPrice;
                _productRepository.UpdateProductPrice(product.ProductId, productPrice);
                _productRepository.SaveChanges();

                var productResponseModel = _mapper.Map<ProductResponse>(product);
                productResponseModel.ProductPrice = productPrice;

                productResponseModels.Add(productResponseModel);
            }

            await _hubContext.Clients.All.SendAsync("ReceiveProducts", productResponseModels);

            return productResponseModels;
        }

        private string GenerateUniqueProductId()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            string letters = new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
            string numbers = random.Next(100000, 999999).ToString("D6");
            return $"{letters}{numbers}";
        }

        public async Task<ProductResponse> CreateProductAsync(AddProductRequest request)
        {
            var productId = GenerateUniqueProductId();
            var product = new Product
            {
                ProductId = productId,
                ProductName = request.ProductName,
                Size = request.Size,
                Img = request.Img,
                CounterId = request.CounterId,
                CategoryId = request.CategoryId,
                MaterialCost = request.MaterialCost,
                DiamondCost = request.DiamondCost,
                ProductionCost = request.ProductionCost,
                Quantity = request.Quantity,
                Status = "Còn hàng",
                PriceRate = request.PriceRate
            };

            await _productRepository.AddAsync(product);
            _productRepository.SaveChanges();


            if (request.Diamonds != null)
            {
                foreach (var diamond in request.Diamonds)
                {
                    var productDiamond = new ProductDiamond
                    {
                        ProductId = productId,
                        DiamondCode = diamond.DiamondCode,
                    };
                    await _productDiamondRepository.AddProductDiamondAsync(productDiamond);
                    _productDiamondRepository.SaveChanges();
                }
            }

            if (request.Materials != null)
            {
                foreach (var material in request.Materials)
                {
                    var productMaterial = new ProductMaterial
                    {
                        ProductId = productId,
                        MaterialId = material.MaterialId,
                        Weight = material.Weight
                    };
                    await _productMaterialRepository.AddProductMaterialAsync(productMaterial);
                    _productMaterialRepository.SaveChanges();
                }
            }

            return new ProductResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Size = product.Size,
                Img = product.Img,
                CounterId = product.CounterId,
                CategoryId = product.CategoryId,
                MaterialCost = product.MaterialCost,
                DiamondCost = product.DiamondCost,
                ProductionCost = product.ProductionCost,
                ProductPrice = product.ProductPrice,
                Quantity = product.Quantity,
                Status = product.Status,
            };
        }



        public async Task<ProductResponse> GetProductById(string productId)
        {
            var productResponse = await _productRepository.GetByIdAsync(productId);
            return _mapper.Map<ProductResponse>(productResponse);
        }

        public async Task<decimal> CalculateBuyBackPriceForSingleProductAsync(string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new Exception($"Product with ID {productId} not found");
            }

            decimal productionCost = product.ProductionCost ?? 0;
            decimal diamondCost = product.DiamondCost ?? 0;
            decimal materialCost = product.MaterialCost ?? 0;

            decimal totalMaterialCost = 0;
            var productMaterials = _productMaterialRepository.GetProductMaterialsByProductId(product.ProductId);
            if (productMaterials != null)
            {
                foreach (var productMaterial in productMaterials)
                {
                    if (productMaterial.MaterialId.HasValue)
                    {
                        var materialPrice = _productMaterialRepository.GetMaterialPriceById(productMaterial.MaterialId.Value);
                        if (materialPrice != null)
                        {
                            totalMaterialCost += materialPrice.BuyPrice * (productMaterial.Weight ?? 0);
                        }
                    }
                }
            }

            decimal totalDiamondCost = 0;
            var productDiamonds = await _productDiamondRepository.GetProductDiamondsByProductIdAsync(product.ProductId);
            if (productDiamonds != null)
            {
                foreach (var productDiamond in productDiamonds)
                {
                    var diamond = productDiamond.DiamondCodeNavigation;
                    if (diamond != null)
                    {
                        var latestPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(diamond.Origin, diamond.CaratWeight, diamond.Color, diamond.Clarity, diamond.Cut);
                        if (latestPrice != null)
                        {
                            totalDiamondCost += latestPrice.BuyPrice ?? 0;
                        }
                    }
                }
            }

            decimal productBuyPrice = totalMaterialCost + productionCost + diamondCost + materialCost + totalDiamondCost;
            decimal priceRatePercent = product.PriceRate ?? 0;
            decimal priceRateDecimal = priceRatePercent / 100;
            decimal buyBackPrice = productBuyPrice * (1 + priceRateDecimal);

            return buyBackPrice;
        }




    }
}
