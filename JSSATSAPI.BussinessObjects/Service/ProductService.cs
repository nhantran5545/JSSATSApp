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
        private readonly IMaterialRepository _materialRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IProductDiamondRepository _productDiamondRepository;
        private readonly IProductMaterialRepository _productMaterialRepository;
        private readonly IMapper _mapper;
        private readonly ICounterRepository _counterRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHubContext<ProductHub> _hubContext;

        public ProductService(IProductRepository productRepository, IDiamondRepository diamondRepository,
            IDiamondPriceRepository diamondPriceRepository, IProductDiamondRepository productDiamondRepository, 
            IProductMaterialRepository productMaterialRepository, IMapper mapper, IHubContext<ProductHub> hubContext, 
            ICounterRepository counterRepository, ICategoryRepository categoryRepository, IMaterialRepository materialRepository)
        {
            _productRepository = productRepository;
            _diamondRepository = diamondRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _productDiamondRepository = productDiamondRepository;
            _productMaterialRepository = productMaterialRepository;
            _materialRepository = materialRepository;
            _mapper = mapper;
            _hubContext = hubContext;
            _counterRepository = counterRepository;
            _categoryRepository = categoryRepository;
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
                decimal totalBuyPriceMaterialCost = 0;
                string materName = null;
                decimal materWeight = 0;
                var categoryDiscountRate = product.Category?.DiscountRate ?? 0;

                var productMaterials = _productMaterialRepository.GetProductMaterialsByProductId(product.ProductId);
                foreach (var productMaterial in productMaterials)
                {
                    var materialPrice = _productMaterialRepository.GetMaterialPriceById(productMaterial.MaterialId.Value);
                    var materialName = productMaterial.Material.MaterialName;
                    var materialWeight = productMaterial.Weight;
                    if (materialPrice != null)
                    {
                        totalMaterialCost += materialPrice.SellPrice * (productMaterial.Weight ?? 0);
                        totalBuyPriceMaterialCost += materialPrice.BuyPrice * (productMaterial.Weight ?? 0);
                        materName = materialName;
                        materialWeight = materialWeight ?? 0;
                    }
                }

                decimal totalDiamondCost = 0;
                decimal totalBuyPriceDiamondCost = 0;
                string diaName = null;
                string diaCode = null;
                var productDiamondsList = productDiamonds.Where(pd => pd.ProductId == product.ProductId);
                foreach (var productDiamond in productDiamondsList)
                {
                    var diamond = productDiamond.DiamondCodeNavigation;
                    var diamondName = productDiamond.DiamondCodeNavigation.DiamondName;
                    var diamondCode = productDiamond.DiamondCodeNavigation.DiamondCode;
                    diaName = diamondName;
                    var latestPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(diamond.Origin, diamond.CaratWeightFrom, diamond.CaratWeightTo, diamond.Color, diamond.Clarity, diamond.Cut);

                    if (latestPrice != null)
                    {
                        totalDiamondCost += latestPrice.SellPrice ?? 0;
                        totalBuyPriceDiamondCost += latestPrice.BuyPrice ?? 0;
                    }
                }


                //SellPrice
                decimal costPrice = totalMaterialCost + totalDiamondCost + productionCost + diamondCost + materialCost;
                //BuyPrice
                decimal costBuyPrice = totalBuyPriceMaterialCost + totalBuyPriceDiamondCost + productionCost + diamondCost + materialCost ;

                decimal priceRatePercent = product.PriceRate ?? 0;
                decimal priceRateDecimal = priceRatePercent / 100;
                //ProductSellPrice
                decimal productPrice = costPrice * (1 + priceRateDecimal);
                //ProductBuyPrice
                decimal productBuyPrice = costBuyPrice * (1 + priceRateDecimal);

                // Cập nhật giá sản phẩm vào database
                product.ProductPrice = productPrice;
                product.BuyBackPrice = productBuyPrice;
                _productRepository.UpdateProductPrice(product.ProductId, productPrice, productBuyPrice);
                _productRepository.SaveChanges();   

                var productResponseModel = _mapper.Map<ProductResponse>(product);
                productResponseModel.ProductPrice = productPrice;
                productResponseModel.BuyBackPrice = productBuyPrice;
                productResponseModel.DiscountRate = categoryDiscountRate;
                productResponseModel.DiamondCode = diaCode ?? "No DiamondCode";
                productResponseModel.DiamondName = diaName ?? "No Diamond";
                productResponseModel.ProductDiamondCost = totalDiamondCost;
                productResponseModel.ProductDiamondCostBuyBack = totalBuyPriceDiamondCost;
                productResponseModel.MaterialName = materName ?? "No Material";
                productResponseModel.MaterialWeight = materWeight;
                productResponseModel.ProductMaterialCost = totalMaterialCost;
                productResponseModel.ProductMaterialCostBuyBack = totalBuyPriceMaterialCost;
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

                var categoryDiscountRate = product.Category?.DiscountRate ?? 0;

                decimal totalMaterialCost = 0;
                decimal totalBuyPriceMaterialCost = 0;
                string materName = null;
                decimal materWeight = 0;
                var productMaterials = _productMaterialRepository.GetProductMaterialsByProductId(product.ProductId);
                foreach (var productMaterial in productMaterials)
                {
                    var materialPrice = _productMaterialRepository.GetMaterialPriceById(productMaterial.MaterialId.Value);
                    var materialName = productMaterial.Material.MaterialName;
                    var materialWeight = productMaterial.Weight;
                    if (materialPrice != null)
                    {
                        totalMaterialCost += materialPrice.SellPrice * (productMaterial.Weight ?? 0);
                        totalBuyPriceMaterialCost += materialPrice.BuyPrice * (productMaterial.Weight ?? 0);
                        materName = materialName;
                        materWeight = materialWeight ?? 0;
                    }
                }

                decimal totalDiamondCost = 0;
                decimal totalBuyPriceDiamondCost = 0;
                string diaName = null;
                string diaCode = null;
                var productDiamondsList = productDiamonds.Where(pd => pd.ProductId == product.ProductId);
                foreach (var productDiamond in productDiamondsList)
                {
                    var diamond = productDiamond.DiamondCodeNavigation;
                    var diamondName = diamond.DiamondName;
                    var diamondCode = diamond.DiamondCode;
                    var latestPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(diamond.Origin, diamond.CaratWeightFrom,diamond.CaratWeightTo, diamond.Color, diamond.Clarity, diamond.Cut);
                    if (latestPrice != null)
                    {
                        diaName = diamondName;
                        diaCode = diamondCode;
                        totalDiamondCost += latestPrice.SellPrice ?? 0;
                        totalBuyPriceDiamondCost += latestPrice.BuyPrice ?? 0;
                    }
                }

                //SellPrice
                decimal costPrice = totalMaterialCost + totalDiamondCost + productionCost + diamondCost + materialCost;
                //BuyPrice
                decimal costBuyPrice = totalBuyPriceMaterialCost + totalBuyPriceDiamondCost + productionCost + diamondCost + materialCost;

                decimal priceRatePercent = product.PriceRate ?? 0;
                decimal priceRateDecimal = priceRatePercent / 100;
                //ProductSellPrice
                decimal productPrice = costPrice * (1 + priceRateDecimal);
                //ProductBuyPrice
                decimal productBuyPrice = costBuyPrice * (1 + priceRateDecimal);

                // Cập nhật giá sản phẩm vào database
                product.ProductPrice = productPrice;
                product.BuyBackPrice = productBuyPrice;
                _productRepository.UpdateProductPrice(product.ProductId, productPrice, productBuyPrice);
                _productRepository.SaveChanges();

                var productResponseModel = _mapper.Map<ProductResponse>(product);
                productResponseModel.ProductPrice = productPrice;
                productResponseModel.BuyBackPrice = productBuyPrice;
                productResponseModel.DiscountRate = categoryDiscountRate;
                productResponseModel.DiamondCode = diaCode ?? "No DiamondCode";
                productResponseModel.DiamondName = diaName ?? "No Diamond";
                productResponseModel.ProductDiamondCost = totalDiamondCost;
                productResponseModel.ProductDiamondCostBuyBack = totalBuyPriceDiamondCost;
                productResponseModel.MaterialName = materName ?? "No Material";
                productResponseModel.MaterialWeight = materWeight;
                productResponseModel.ProductMaterialCost = totalMaterialCost;
                productResponseModel.ProductMaterialCostBuyBack = totalBuyPriceMaterialCost;
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
            var counterExists = await _counterRepository.GetByIdAsync(request.CounterId);
            if (counterExists == null)
            {
                throw new Exception($"Counter with ID {request.CounterId} not found");
            }

            var categoryExists = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (categoryExists == null)
            {
                throw new Exception($"Category with ID {request.CategoryId} not found");
            }

            // Generate unique Product ID
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
                Quantity = 1,
                Status = "Còn hàng",
                PriceRate = request.PriceRate
            };

            await _productRepository.AddAsync(product);
            _productRepository.SaveChanges();

            var productDiamonds = new List<ProductDiamond>();
            var diamondNames = new List<string>();

            // Validate and add Diamonds
            if (request.Diamonds != null && request.Diamonds.Any())
            {
                foreach (var diamond in request.Diamonds)
                {
                    var diamondExists = await _diamondRepository.GetByIdAsync(diamond.DiamondCode);
                    if (diamondExists == null)
                    {
                        throw new Exception($"Diamond with code {diamond.DiamondCode} not found");
                    }

                    var productDiamond = new ProductDiamond
                    {
                        ProductId = productId,
                        DiamondCode = diamond.DiamondCode,
                    };
                    await _productDiamondRepository.AddProductDiamondAsync(productDiamond);
                    productDiamonds.Add(productDiamond);
                    diamondNames.Add(diamondExists.DiamondName);

                    // Update Diamond status to "InActive"
                    diamondExists.Status = "InActive";
                    _diamondRepository.Update(diamondExists);
                }
                _productDiamondRepository.SaveChanges();
                _diamondRepository.SaveChanges();
            }

            var productMaterials = new List<ProductMaterial>();
            var materialNames = new List<string>();

            // Validate and add Materials
            if (request.Materials != null && request.Materials.Any())
            {
                foreach (var material in request.Materials)
                {
                    var materialExists = await _materialRepository.GetByIdAsync(material.MaterialId);
                    if (materialExists == null)
                    {
                        throw new Exception($"Material with ID {material.MaterialId} not found");
                    }

                    var productMaterial = new ProductMaterial
                    {
                        ProductId = productId,
                        MaterialId = material.MaterialId,
                        Weight = material.Weight
                    };
                    await _productMaterialRepository.AddProductMaterialAsync(productMaterial);
                    productMaterials.Add(productMaterial);
                    materialNames.Add(materialExists.MaterialName);
                }
                _productMaterialRepository.SaveChanges();
            }

            return new ProductResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Size = product.Size,
                Img = product.Img,
                CounterId = product.CounterId,
                CounterName = counterExists.CounterName,
                CategoryId = product.CategoryId,
                CategoryName = categoryExists.CategoryName,
                MaterialCost = product.MaterialCost,
                DiamondCost = product.DiamondCost,
                ProductionCost = product.ProductionCost,
                ProductPrice = product.ProductPrice,
                Quantity = product.Quantity,
                Status = product.Status,
                MaterialName = string.Join(", ", materialNames),
                DiamondName = string.Join(", ", diamondNames),
            };
        }


        public async Task<ProductResponse> GetProductByIdAsync(string productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);

            if (product == null)
            {
                return null;
            }
            decimal totalMaterialCost = 0;
            decimal totalBuyPriceMaterialCost = 0;
            string materName = null;
            decimal materWeight = 0;
            var categoryDiscountRate = product.Category?.DiscountRate ?? 0;

            var productMaterials = _productMaterialRepository.GetProductMaterialsByProductId(product.ProductId);
            foreach (var productMaterial in productMaterials)
            {
                var materialPrice = _productMaterialRepository.GetMaterialPriceById(productMaterial.MaterialId.Value);
                var materialName = productMaterial.Material.MaterialName;
                var materialWeight = productMaterial.Weight;
                if (materialPrice != null)
                {
                    totalMaterialCost += materialPrice.SellPrice * (productMaterial.Weight ?? 0);
                    totalBuyPriceMaterialCost += materialPrice.BuyPrice * (productMaterial.Weight ?? 0);
                    materName = materialName;
                    materWeight = materialWeight ?? 0;
                }
            }
            decimal totalDiamondCost = 0;
            decimal totalBuyPriceDiamondCost = 0;
            string diaName = null;
            string diaCode = null;
            var productDiamondsList = _productDiamondRepository.GetProductDiamondsByProductId(product.ProductId);
            foreach (var productDiamond in productDiamondsList)
            {
                var diamond = productDiamond.DiamondCodeNavigation;
                var diamondName = productDiamond.DiamondCodeNavigation.DiamondName;
                var diamondCode = productDiamond.DiamondCodeNavigation.DiamondCode;
                var latestPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(diamond.Origin, diamond.CaratWeightFrom, diamond.CaratWeightTo, diamond.Color, diamond.Clarity, diamond.Cut);
                if (latestPrice != null)
                {
                    diaCode = diamondCode;
                    diaName = diamondName;
                    totalDiamondCost += latestPrice.SellPrice ?? 0;
                    totalBuyPriceDiamondCost += latestPrice.BuyPrice ?? 0;
                }
            }




            var productResponseModel = _mapper.Map<ProductResponse>(product);
            productResponseModel.DiscountRate = categoryDiscountRate;
            productResponseModel.DiamondCode = diaCode ?? "No DiamondCode";
            productResponseModel.DiamondName = diaName ?? "No Diamond";
            productResponseModel.ProductDiamondCost = totalDiamondCost;
            productResponseModel.ProductDiamondCostBuyBack = totalBuyPriceDiamondCost;
            productResponseModel.MaterialName = materName ?? "No Material";
            productResponseModel.MaterialWeight = materWeight;
            productResponseModel.ProductMaterialCost = totalMaterialCost;
            productResponseModel.ProductMaterialCostBuyBack = totalBuyPriceMaterialCost;

            return productResponseModel;
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
                        var latestPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(diamond.Origin, diamond.CaratWeightFrom, diamond.CaratWeightTo, diamond.Color, diamond.Clarity, diamond.Cut);
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

            product.BuyBackPrice = buyBackPrice;
             _productRepository.Update(product);
             _productRepository.SaveChanges();

            return buyBackPrice;
        }

        public async Task<ProductResponse> UpdateProductAsync(string productId, UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            var counterExists = await _counterRepository.GetByIdAsync(request.CounterId);
            if (counterExists == null)
            {
                throw new Exception($"Counter with ID {request.CounterId} not found");
            }

            var categoryExists = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (categoryExists == null)
            {
                throw new Exception($"Category with ID {request.CategoryId} not found");
            }

            // Update product details only if provided in the request
            if (request.ProductName != null)
            {
                product.ProductName = request.ProductName;
            }

            if (request.Size != null)
            {
                product.Size = request.Size;
            }

            if (request.Img != null)
            {
                product.Img = request.Img;
            }

            if (request.CounterId != null)
            {
                product.CounterId = request.CounterId;
            }

            if (request.CategoryId != null)
            {
                product.CategoryId = request.CategoryId;
            }

            if (request.MaterialCost != null)
            {
                product.MaterialCost = request.MaterialCost.Value;
            }

            if (request.DiamondCost != null)
            {
                product.DiamondCost = request.DiamondCost.Value;
            }

            if (request.ProductionCost != null)
            {
                product.ProductionCost = request.ProductionCost.Value;
            }

            if (request.PriceRate != null)
            {
                product.PriceRate = request.PriceRate.Value;
            }

            _productRepository.Update(product);
            _productRepository.SaveChanges();

            // Update Diamonds if provided
            if (request.Diamonds != null && request.Diamonds.Any())
            {
                var existingDiamonds = await _productDiamondRepository.GetDiamondsByProductIdAsync(productId);

                foreach (var existingDiamond in existingDiamonds)
                {
                    var diamond = await _diamondRepository.GetByIdAsync(existingDiamond.DiamondCode);
                    if (diamond != null)
                    {
                        diamond.Status = "Active";
                        _diamondRepository.Update(diamond);
                    }
                }

                await _productDiamondRepository.DeleteProductDiamondsByProductIdAsync(productId);

                foreach (var diamond in request.Diamonds)
                {
                    var diamondExists = await _diamondRepository.GetByIdAsync(diamond.DiamondCode);
                    if (diamondExists == null)
                    {
                        throw new Exception($"Diamond with code {diamond.DiamondCode} not found");
                    }

                    var productDiamond = new ProductDiamond
                    {
                        ProductId = productId,
                        DiamondCode = diamond.DiamondCode,
                    };

                    await _productDiamondRepository.AddProductDiamondAsync(productDiamond);

                    // Inactivate new diamonds
                    diamondExists.Status = "InActive";
                    _diamondRepository.Update(diamondExists);
                }
                _productDiamondRepository.SaveChanges();
                _diamondRepository.SaveChanges();
            }

            // Update Materials if provided
            if (request.Materials != null && request.Materials.Any())
            {
                await _productMaterialRepository.DeleteProductMaterialsByProductIdAsync(productId);
                foreach (var material in request.Materials)
                {
                    var materialExists = await _materialRepository.GetByIdAsync(material.MaterialId);
                    if (materialExists == null)
                    {
                        throw new Exception($"Material with ID {material.MaterialId} not found");
                    }

                    var productMaterial = new ProductMaterial
                    {
                        ProductId = productId,
                        MaterialId = material.MaterialId,
                        Weight = material.Weight
                    };
                    await _productMaterialRepository.AddProductMaterialAsync(productMaterial);
                }
                _productMaterialRepository.SaveChanges();
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


    }
}
