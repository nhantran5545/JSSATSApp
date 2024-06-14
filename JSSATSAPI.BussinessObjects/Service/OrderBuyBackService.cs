using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest;
using JSSATSAPI.BussinessObjects.ResponseModels;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackDetailRes;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse;
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
        private readonly IOrderSellDetailRepository _orderSellDetail;
        private readonly IProductRepository _productRepository;
        private readonly IMaterialPriceRepository _materialPriceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductMaterialRepository _productMaterialRepository;
        private readonly IMaterialRepository _materialRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IAccountService _accountService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public OrderBuyBackService(IOrderBuyBackRepository orderBuyBackRepository,
            IProductRepository productRepository, IMaterialPriceRepository materialPriceRepository,
            IProductMaterialRepository productMaterialRepository,
            IMaterialRepository materialRepository,
            IDiamondPriceRepository diamondPriceRepository, IMapper mapper,
            IAccountService accountService, IProductService productService, ICustomerRepository customerRepository, IPaymentRepository paymentRepository, IOrderSellDetailRepository orderSellDetail)
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
            _materialRepository = materialRepository;
            _paymentRepository = paymentRepository;
            _orderSellDetail = orderSellDetail;
        }

        public async Task<List<OrderBuyBackBothResponse>> GetAllOrderBuyBacksAsync()
        {
            var orderBuyBacks = await _orderBuyBackRepository.GetAllAsync();
            var orderBuyBackResponses = _mapper.Map<List<OrderBuyBackBothResponse>>(orderBuyBacks);
            return orderBuyBackResponses;
        }

        public async Task<OrderBuyBackBothResponse> GetOrderBuyBackById(int orderBuyBackId)
        {
            var orderBuyBack = await _orderBuyBackRepository.GetByIdAsync(orderBuyBackId);
            var orderBuyBackResponse = _mapper.Map<OrderBuyBackBothResponse>(orderBuyBack);

            return orderBuyBackResponse;
        }

        public async Task<OrderBuyBackResponse> BuyBackProductOutOfStoreAsync(OrderBuyBackRequest request)
        {
            var orderBuyBack = new OrderBuyBack
            {
                CustomerId = request.CustomerId,
                Status = "Processing",
                DateBuyBack = DateTime.Now,
                OrderBuyBackDetails = new List<OrderBuyBackDetail>()
            };

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            decimal totalAmount = 0;
            var detailResponses = new List<OrderBuyBackDetailResponse>();

            foreach (var detail in request.OrderBuyBackDetails)
            {
                decimal price = await CalculateDetailPriceAsync(detail);

                var orderBuyBackDetail = new OrderBuyBackDetail
                {
                    BuyBackProductName = detail.BuyBackProductName ?? null,
                    MaterialId = detail.MaterialId ?? null,
                    Quantity =  1,
                    Weight = detail.Weight ?? null, 
                    Origin = detail.Origin ?? null,
                    CaratWeight = detail.CaratWeight ?? null, 
                    Color = detail.Color ?? null, 
                    Clarity = detail.Clarity ?? null,
                    Cut = detail.Cut ?? null, 
                    Price = price
                };

                orderBuyBack.OrderBuyBackDetails.Add(orderBuyBackDetail);
                totalAmount += price;

                var detailResponse = _mapper.Map<OrderBuyBackDetailResponse>(orderBuyBackDetail);
                detailResponses.Add(detailResponse);
            }

            orderBuyBack.TotalAmount = totalAmount;
            orderBuyBack.FinalAmount = totalAmount;

            await _orderBuyBackRepository.AddAsync(orderBuyBack);
            _orderBuyBackRepository.SaveChanges();

            var response = new OrderBuyBackResponse
            {
                OrderBuyBackId = orderBuyBack.OrderBuyBackId,
                CustomerId = customer.CustomerId,
                CustomerName = customer.Name,
                CustomerPhone = customer.Phone,
                DateBuyBack = orderBuyBack.DateBuyBack,
                TotalAmount = orderBuyBack.TotalAmount,
                FinalAmount = orderBuyBack.FinalAmount,
                Status = orderBuyBack.Status,
                OrderBuyBackDetails = detailResponses,
                Payments = new List<PaymentResponse>()
            };

            return response;
        }


        private async Task<decimal> CalculateDetailPriceAsync(OrderBuyBackDetailRequest detail)
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
                    throw new InvalidOperationException("Material with ID " + detail.MaterialId.Value + " not found.");
                }
            }

            if (!string.IsNullOrEmpty(detail.Origin) && detail.CaratWeight.HasValue && !string.IsNullOrEmpty(detail.Color) &&
                !string.IsNullOrEmpty(detail.Clarity) && !string.IsNullOrEmpty(detail.Cut))
            {
                var diamondPrice = await _diamondPriceRepository.GetBuyPriceDiamondPriceAsync(
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
                    throw new InvalidOperationException("Diamond price not found for origin " + detail.Origin + ", carat weight " + detail.CaratWeight + ", color " + detail.Color + ", clarity " + detail.Clarity + ", cut " + detail.Cut + ".");
                }
            }

            return price;
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
            decimal buyBackPriceTotal = 0;
            decimal productPriceTotal = 0;
            decimal discountRate = 0;

            foreach (var productId in request.ProductIds)
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {productId} not found");
                }
                var buyBackPrice = await _productService.CalculateBuyBackPriceForSingleProductAsync(productId);

                var productPrice = product.ProductPrice ?? 0;
                var categoryDiscountRate = product.Category?.DiscountRate ?? 0;
                var discountRateDecimal = categoryDiscountRate / 100;

                var discountPrice = buyBackPrice + ((productPrice - buyBackPrice) * discountRateDecimal);

                buyBackPriceTotal += buyBackPrice;
                productPriceTotal += productPrice;
                discountRate += categoryDiscountRate;
                totalAmount += discountPrice;

                var orderBuyBackDetail = new OrderBuyBackDetail
                {
                    ProductId = productId,
                    Price = discountPrice,
                };

                orderBuyBackDetails.Add(orderBuyBackDetail);
            }

            var orderBuyBack = new OrderBuyBack
            {
                CustomerId = request.CustomerId,
                DateBuyBack = DateTime.UtcNow,
                TotalAmount = totalAmount,
                FinalAmount = totalAmount,
                Status = "Processing",
                OrderBuyBackDetails = orderBuyBackDetails
            };

            await _orderBuyBackRepository.AddAsync(orderBuyBack);
            _orderBuyBackRepository.SaveChanges();


            var response = _mapper.Map<OrderBuyBackInStoreResponse>(orderBuyBack);
            response.BuyBackPriceTotal = buyBackPriceTotal;
            response.ProductPriceTotal = productPriceTotal;
            response.DiscountRate = discountRate;


            return response;
        }


        public async Task<OrderBuyBackBothResponse> PayForBuyBackAsync(PaidOrderBuyBackReq request)
        {
            var orderBuyBack = await _orderBuyBackRepository.GetByIdAsync(request.OrderBuyBackId);
            if (orderBuyBack == null)
            {
                throw new Exception("Order buyback not found");
            }

            foreach (var paymentReq in request.Payments)
            {
                if (paymentReq.PaymentTypeId != 2 && paymentReq.PaymentTypeId != 3)
                {
                    throw new InvalidOperationException("Invalid PaymentType. PaymentType should be 2 (Tiền mặt) or 3 (Chuyển Khoản)");
                }

                var payment = _mapper.Map<Payment>(paymentReq);
                payment.CreateDate = DateTime.Now;
                payment.OrderBuyBackId = request.OrderBuyBackId;
                payment.Amount = orderBuyBack.FinalAmount ?? 0;
                await _paymentRepository.AddAsync(payment);
            }

            orderBuyBack.Status = "Paid";
            _orderBuyBackRepository.Update(orderBuyBack);
            _orderBuyBackRepository.SaveChanges();

            var response = _mapper.Map<OrderBuyBackBothResponse>(orderBuyBack);
            response.CustomerName = orderBuyBack.Customer.Name;
            response.CustomerPhone = orderBuyBack.Customer.Phone;
            return response;
        }

        public async Task<OrderBuyBackBothResponse> CancelOrderBuyBackAsync(int orderBbId)
        {
            var orderBb = await _orderBuyBackRepository.GetByIdAsync(orderBbId);
            if (orderBb == null)
            {
                throw new Exception("Order not found");
            }

            if (orderBb.Status == "Paid")
            {
                throw new InvalidOperationException("Orders cannot be canceled once payment has been made.");
            }


            orderBb.Status = "Cancelled";
            _orderBuyBackRepository.Update(orderBb);
            _orderBuyBackRepository.SaveChanges();

            var response = _mapper.Map<OrderBuyBackBothResponse>(orderBb);
            var customer = await _customerRepository.GetByIdAsync(orderBb.CustomerId);
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
                        throw new InvalidOperationException("Material with ID" + detail.MaterialId.Value + "not found.");
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
                            throw new InvalidOperationException("Origin is missing for diamond detail.");
                        if (!detail.CaratWeight.HasValue)
                            throw new InvalidOperationException("CaratWeight is missing for diamond detail.");
                        if (string.IsNullOrEmpty(detail.Color))
                            throw new InvalidOperationException("Color is missing for diamond detail.");
                        if (string.IsNullOrEmpty(detail.Clarity))
                            throw new InvalidOperationException("Clarity is missing for diamond detail.");
                        if (string.IsNullOrEmpty(detail.Cut))
                            throw new InvalidOperationException("Cut is missing for diamond detail.");
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
            var diamondPrice = await _diamondPriceRepository.GetBuyPriceDiamondPriceAsync(origin, caratWeight, color, clarity, cut);
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
