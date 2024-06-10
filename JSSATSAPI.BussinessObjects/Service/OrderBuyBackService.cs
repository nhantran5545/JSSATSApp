using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackDetailRes;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackResponse;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using JSSATSAPI.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class OrderBuyBackService : IOrderBuyBackService
    {
        private readonly IOrderBuyBackRepository _orderBuyBackRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMaterialPriceRepository _materialPriceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductMaterialRepository _productMaterialRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IAccountService _accountService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public OrderBuyBackService(IOrderBuyBackRepository orderBuyBackRepository, 
            IProductRepository productRepository, IMaterialPriceRepository materialPriceRepository,
            IProductMaterialRepository productMaterialRepository,
            IDiamondPriceRepository diamondPriceRepository, IMapper mapper,
            IAccountService accountService, IProductService productService, ICustomerRepository customerRepository)
        {
            _orderBuyBackRepository = orderBuyBackRepository;
            _productRepository = productRepository;
            _materialPriceRepository = materialPriceRepository;
            _productMaterialRepository = productMaterialRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _mapper = mapper;
            _accountService = accountService;
            _productService = productService;
            _customerRepository = customerRepository;
        }
        public async Task<OrderBuyBackResponse> BuyBackProductOutOfStoreAsync(OrderBuyBackRequest request)
        {
            var orderBuyBack = _mapper.Map<OrderBuyBack>(request);
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            decimal totalAmount = 0;
            var detailResponses = new List<OrderBuyBackDetailResponse>();
            var errorMessages = new List<string>();

            foreach (var detail in request.OrderBuyBackDetails)
            {
                decimal price = 0;

                if (detail.MaterialId.HasValue)
                {
                    var materialPrice = await _productMaterialRepository.GetLatestMaterialPriceAsync(detail.MaterialId.Value);
                    if (materialPrice != null)
                    {
                        price += materialPrice.BuyPrice * (detail.Weight ?? 0);
                    }
                    else
                    {
                        errorMessages.Add($"Material with ID {detail.MaterialId.Value} not found.");
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(detail.Origin) || detail.CaratWeight.HasValue || !string.IsNullOrEmpty(detail.Color) ||
                    !string.IsNullOrEmpty(detail.Clarity) || !string.IsNullOrEmpty(detail.Cut))
                {
                    if (!string.IsNullOrEmpty(detail.Origin) && detail.CaratWeight.HasValue && !string.IsNullOrEmpty(detail.Color) &&
                        !string.IsNullOrEmpty(detail.Clarity) && !string.IsNullOrEmpty(detail.Cut))
                    {
                        var diamondPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(
                            detail.Origin,
                            detail.CaratWeight.Value,
                            detail.Color,
                            detail.Clarity,
                            detail.Cut
                        );
                        if (diamondPrice != null)
                        {
                            price += diamondPrice.BuyPrice ?? 0;
                        }
                        else
                        {
                            errorMessages.Add($"Diamond price not found for origin {detail.Origin}, carat weight {detail.CaratWeight}, color {detail.Color}, clarity {detail.Clarity}, cut {detail.Cut}.");
                            continue;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(detail.Origin))
                            errorMessages.Add("Origin is missing for diamond detail.");
                        if (!detail.CaratWeight.HasValue)
                            errorMessages.Add("Carat weight is missing for diamond detail.");
                        if (string.IsNullOrEmpty(detail.Color))
                            errorMessages.Add("Color is missing for diamond detail.");
                        if (string.IsNullOrEmpty(detail.Clarity))
                            errorMessages.Add("Clarity is missing for diamond detail.");
                        if (string.IsNullOrEmpty(detail.Cut))
                            errorMessages.Add("Cut is missing for diamond detail.");
                        continue;
                    }
                }

                var orderBuyBackDetail = _mapper.Map<OrderBuyBackDetail>(detail);
                orderBuyBackDetail.Price = price;

                orderBuyBack.OrderBuyBackDetails.Add(orderBuyBackDetail);
                totalAmount += price;

                detailResponses.Add(_mapper.Map<OrderBuyBackDetailResponse>(orderBuyBackDetail));
            }

            if (errorMessages.Any())
            {
                return new OrderBuyBackResponse
                {
                    Errors = errorMessages
                };
            }

            orderBuyBack.TotalAmount = totalAmount;
            orderBuyBack.FinalAmount = totalAmount;
            orderBuyBack.Status = "Pending";
            orderBuyBack.DateBuyBack = DateTime.Now;
            await _orderBuyBackRepository.AddAsync(orderBuyBack);
             _orderBuyBackRepository.SaveChanges();

            var response = _mapper.Map<OrderBuyBackResponse>(orderBuyBack);
            response.OrderBuyBackDetails = detailResponses;
            response.CustomerName = customer.Name;
            response.CustomerPhone = customer.Phone;
            return response;
        }


        public async Task<OrderBuyBackInStoreResponse> CreateOrderBuyBackInStoreAsync(OrderBuyBackInStoreRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            var orderBuyBackDetails = new List<OrderBuyBackDetail>();
            decimal totalAmount = 0;

            foreach (var productId in request.ProductIds)
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {productId} not found");
                }

                var buyBackPrice = await _productService.CalculateBuyBackPriceForSingleProductAsync(productId);
                totalAmount += buyBackPrice;

                var orderBuyBackDetail = new OrderBuyBackDetail
                {
                    ProductId = productId,
                    Price = buyBackPrice,
                };

                orderBuyBackDetails.Add(orderBuyBackDetail);
            }

            var finalAmount = totalAmount; 

            var orderBuyBack = new OrderBuyBack
            {
                CustomerId = request.CustomerId,
                DateBuyBack = DateTime.UtcNow,
                TotalAmount = totalAmount,
                FinalAmount = finalAmount,
                Status = "Processing",
                OrderBuyBackDetails = orderBuyBackDetails
            };

            await _orderBuyBackRepository.AddAsync(orderBuyBack);
            _orderBuyBackRepository.SaveChanges();

            var response = _mapper.Map<OrderBuyBackInStoreResponse>(orderBuyBack);
            return response;
        }


        public async Task<CalculatePricesResponse> CalculatePricesAsync(OrderBuyBackRequest request)
        {
            decimal totalAmount = 0;
            var detailResponses = new List<OrderBuyBackDetailResponse>();
            var errorMessages = new List<string>();

            foreach (var detail in request.OrderBuyBackDetails)
            {
                decimal price = 0;

                if (detail.MaterialId.HasValue)
                {
                    var materialPriceResult = await CalculateMaterialPriceAsync(detail.MaterialId.Value, detail.Weight ?? 0);
                    if (materialPriceResult.Success)
                    {
                        price += materialPriceResult.Price;
                    }
                    else
                    {
                        errorMessages.Add(materialPriceResult.ErrorMessage);
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(detail.Origin) || detail.CaratWeight.HasValue || !string.IsNullOrEmpty(detail.Color) ||
                    !string.IsNullOrEmpty(detail.Clarity) || !string.IsNullOrEmpty(detail.Cut))
                {
                    if (!string.IsNullOrEmpty(detail.Origin) && detail.CaratWeight.HasValue && !string.IsNullOrEmpty(detail.Color) &&
                        !string.IsNullOrEmpty(detail.Clarity) && !string.IsNullOrEmpty(detail.Cut))
                    {
                        var diamondPriceResult = await CalculateDiamondPriceAsync(
                            detail.Origin,
                            detail.CaratWeight.Value,
                            detail.Color,
                            detail.Clarity,
                            detail.Cut
                        );
                        if (diamondPriceResult.Success)
                        {
                            price += diamondPriceResult.Price;
                        }
                        else
                        {
                            errorMessages.Add(diamondPriceResult.ErrorMessage);
                            continue;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(detail.Origin))
                            errorMessages.Add("Origin is missing for diamond detail.");
                        if (!detail.CaratWeight.HasValue)
                            errorMessages.Add("Carat weight is missing for diamond detail.");
                        if (string.IsNullOrEmpty(detail.Color))
                            errorMessages.Add("Color is missing for diamond detail.");
                        if (string.IsNullOrEmpty(detail.Clarity))
                            errorMessages.Add("Clarity is missing for diamond detail.");
                        if (string.IsNullOrEmpty(detail.Cut))
                            errorMessages.Add("Cut is missing for diamond detail.");
                        continue;
                    }
                }

                totalAmount += price;

                var orderBuyBackDetailResponse = new OrderBuyBackDetailResponse
                {
                    MaterialId = detail.MaterialId,
                    Weight = detail.Weight,
                    Origin = detail.Origin,
                    CaratWeight = detail.CaratWeight,
                    Color = detail.Color,
                    Clarity = detail.Clarity,
                    Cut = detail.Cut,
                    Price = price
                };

                detailResponses.Add(orderBuyBackDetailResponse);
            }

            if (errorMessages.Any())
            {
                return new CalculatePricesResponse
                {
                    Errors = errorMessages
                };
            }

            return new CalculatePricesResponse
            {
                TotalAmount = totalAmount,
                OrderBuyBackDetails = detailResponses
            };
        }

        public async Task<PriceCalculationResult> ReviewMaterialPriceAsync(int materialId, decimal weight)
        {
            return await CalculateMaterialPriceAsync(materialId, weight);
        }

        public async Task<PriceCalculationResult> ReviewDiamondPriceAsync(string origin, decimal caratWeight, string color, string clarity, string cut)
        {
            return await CalculateDiamondPriceAsync(origin, caratWeight, color, clarity, cut);
        }

        private async Task<PriceCalculationResult> CalculateMaterialPriceAsync(int materialId, decimal weight)
        {
            var materialPrice = await _productMaterialRepository.GetLatestMaterialPriceAsync(materialId);
            if (materialPrice != null)
            {
                return new PriceCalculationResult
                {
                    Success = true,
                    Price = materialPrice.BuyPrice * weight
                };
            }
            return new PriceCalculationResult
            {
                Success = false,
                ErrorMessage = $"Material with ID {materialId} not found."
            };
        }

        private async Task<PriceCalculationResult> CalculateDiamondPriceAsync(string origin, decimal caratWeight, string color, string clarity, string cut)
        {
            var diamondPrice = await _diamondPriceRepository.GetLatestDiamondPriceAsync(origin, caratWeight, color, clarity, cut);
            if (diamondPrice != null)
            {
                return new PriceCalculationResult
                {
                    Success = true,
                    Price = diamondPrice.BuyPrice ?? 0
                };
            }
            return new PriceCalculationResult
            {
                Success = false,
                ErrorMessage = $"Diamond price not found for origin {origin}, carat weight {caratWeight}, color {color}, clarity {clarity}, cut {cut}."
            };
        }
    }

}
