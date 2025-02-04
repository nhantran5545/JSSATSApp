﻿using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.OrderSellReq;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using JSSATSAPI.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class OrderSellService : IOrderSellService
    {
        private readonly IOrderSellRepository _orderSellRepository;
        private readonly IOrderSellDetailRepository _orderSellDetailRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountService _accountService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IWarrantyTicketRepository _warrantyTicketRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public OrderSellService(IOrderSellRepository orderSellRepository, 
            IOrderSellDetailRepository orderSellDetailRepository, ICustomerRepository customerRepository, 
            IAccountRepository accountRepository, IMapper mapper, IProductRepository productRepository,
            IPaymentRepository paymentRepository, IWarrantyTicketRepository warrantyTicketRepository, IAccountService accountService, IConfiguration configuration)
        {
            _orderSellRepository = orderSellRepository;
            _orderSellDetailRepository = orderSellDetailRepository;
            _paymentRepository = paymentRepository;
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _productRepository = productRepository;
            _warrantyTicketRepository = warrantyTicketRepository;
            _mapper = mapper;
            _accountService = accountService;
            _configuration = configuration;
        }
        //Create Sell Order
        public OrderSellResponse CreateSellOrder(OrderSellRequest request)
        {
            var orderSellDetails = request.OrderSellDetails.Select(detail => new OrderSellDetail
            {
                ProductId = detail.ProductId,
                Quantity = 1
            }).ToList();

            decimal totalAmount = 0;

            foreach (var orderSellDetail in orderSellDetails)
            {
                var product = _orderSellRepository.GetProductById(orderSellDetail.ProductId);
                if (product == null || product.Quantity < orderSellDetail.Quantity)
                {
                    throw new Exception("Product not available or insufficient quantity");
                }

                orderSellDetail.Price = product.ProductPrice * orderSellDetail.Quantity;

                if (orderSellDetail.Price.HasValue)
                {
                    totalAmount += orderSellDetail.Price.Value;
                }
                product.Status = "Chờ thanh toán";
                product.Quantity -= orderSellDetail.Quantity;
                _productRepository.Update(product);
            }

            if (orderSellDetails.Count == 0)
            {
                throw new Exception("No items in the order");
            }

            var customer = _customerRepository.GetCustomerById(request.CustomerId);
            decimal membershipDiscount = 0;
            decimal? tierDiscountPercent = null;

            if (customer?.Tier != null)
            {
                tierDiscountPercent = customer.Tier.DiscountPercent;
                membershipDiscount = totalAmount * (tierDiscountPercent ?? 0) / 100;
            }

/*            decimal individualPromotionDiscountPercent = request.InvidualPromotionDiscount ?? 0;
            decimal individualPromotionDiscountAmount = totalAmount * individualPromotionDiscountPercent   / 100;*/
            var finalAmount = totalAmount - membershipDiscount;

            int sellerId = _accountService.GetAccountIdFromToken();

            string status = string.IsNullOrEmpty(request.PromotionReason)
                            ? "Processing"
                            : "Approval";

            var orderSell = new OrderSell
            {
                CustomerId = request.CustomerId,
                SellerId = sellerId,
                TotalAmount = totalAmount,
/*              InvidualPromotionDiscount = individualPromotionDiscountAmount,*/
                PromotionReason = request.PromotionReason ?? string.Empty,
                MemberShipDiscount = membershipDiscount,
                FinalAmount = finalAmount,
                OrderDate = DateTime.Now,
                Status = status,
                OrderSellDetails = orderSellDetails,
            };

            _orderSellRepository.AddOrderSell(orderSell);
            _orderSellRepository.SaveChanges();

            var seller = _accountRepository.GetAccountById(sellerId);
            var orderSellResponse = _mapper.Map<OrderSellResponse>(orderSell);
            orderSellResponse.SellerFirstName = seller.FirstName;
            orderSellResponse.SellerLastName = seller.LastName;
            return orderSellResponse;
        }




        //Manager update IndividualPromotion
        public async Task UpdateIndividualPromotionDiscountAsync(int orderSellId, decimal? newDiscount)
        {
            var orderSell = await _orderSellRepository.GetByIdAsync(orderSellId);
            if (orderSell == null)
            {
                throw new Exception("Order not found");
            }
            if (newDiscount.HasValue)
            {
                decimal newDiscountAmount = (orderSell.TotalAmount * newDiscount / 100 ?? 0);

                orderSell.FinalAmount = orderSell.TotalAmount - newDiscountAmount - (orderSell.MemberShipDiscount ?? 0);
                orderSell.InvidualPromotionDiscount = newDiscountAmount;
                orderSell.Status = "Approved";
            }
            _orderSellRepository.Update(orderSell);
             _orderSellRepository.SaveChanges();
        }

        public async Task<OrderSellResponse> PaidOrderSellAsync(CompletedOrderSellResponse completedOrderSellDto)
        {
            var orderSell = await _orderSellRepository.GetByIdAsync(completedOrderSellDto.OrderSellId);
            if (orderSell == null)
            {
                return null;
            }

            if (orderSell.Status == "Cancelled" || orderSell.Status == "Delivered")
            {
                throw new InvalidOperationException("Cannot mark a cancelled or delivered order as paid.");
            }

            if (orderSell.Status == "Paid")
            {
                throw new InvalidOperationException("Order has already been paid.");
            }

            foreach (var paymentDto in completedOrderSellDto.Payments)
            {
                var payment = _mapper.Map<Payment>(paymentDto);
                payment.OrderSellId = completedOrderSellDto.OrderSellId;
                payment.Amount = orderSell.FinalAmount ?? 0;
                await _paymentRepository.AddPaymentAsync(payment);
            }

            foreach (var orderSellDetail in orderSell.OrderSellDetails)
            {
                var product = await _productRepository.GetByIdAsync(orderSellDetail.ProductId);
                if (product != null)
                {
                    product.Status = "Hết hàng";
                    _productRepository.Update(product);
                }
            }

            orderSell.Status = "Paid";
            _orderSellRepository.Update(orderSell);
            _orderSellRepository.SaveChanges();

            var orderSellResponse = _mapper.Map<OrderSellResponse>(orderSell);
            var customer = await _customerRepository.GetByIdAsync(orderSell.CustomerId);
            orderSellResponse.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            return orderSellResponse;
        }

        public async Task<OrderSellResponse> CancelOrderSellAsync(int orderSellId)
        {
            var orderSell = await _orderSellRepository.GetByIdAsync(orderSellId);
            if (orderSell == null)
            {
                throw new Exception("Order not found");
            }

            if (orderSell.Status == "Paid")
            {
                throw new InvalidOperationException("Orders cannot be canceled once payment has been made.");
            }

            if (orderSell.Status == "Delivered")
            {
                throw new InvalidOperationException("Orders cannot be canceled once delivered to the customer.");
            }

            foreach (var orderSellDetail in orderSell.OrderSellDetails)
            {
                var product = await _productRepository.GetByIdAsync(orderSellDetail.ProductId);
                if (product != null)
                {
                    product.Quantity += orderSellDetail.Quantity ?? 0;
                    product.Status = "Còn hàng";
                    _productRepository.Update(product);
                }
            }

            orderSell.Status = "Cancelled";
            _orderSellRepository.Update(orderSell);
            _orderSellRepository.SaveChanges();

            var orderSellResponse = _mapper.Map<OrderSellResponse>(orderSell);
            var customer = await _customerRepository.GetByIdAsync(orderSell.CustomerId);
            orderSellResponse.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            return orderSellResponse;
        }

        public async Task<OrderSellResponse> DeliveredOrderSellAsync(int orderSellId)
        {
            var orderSell = await _orderSellRepository.GetByIdAsync(orderSellId);
            if (orderSell == null)
            {
                throw new InvalidOperationException("Order cannot be marked as Delivered or Cancelled as it is already Delivered or Paid.");
            }

            // Check if the order status is already Delivered or Paid
            if (orderSell.Status == "Cancelled")
            {
                throw new InvalidOperationException("Order cannot be cancelled.");
            }

            if (orderSell.Status == "Delivered")
            {
                throw new InvalidOperationException("Order has already been delivered.");
            }

            orderSell.Status = "Delivered";
            _orderSellRepository.Update(orderSell);
            _orderSellRepository.SaveChanges();

            // Update customer loyaltyPoints
            var customer = await _customerRepository.GetByIdAsync(orderSell.CustomerId);
            if (customer != null)
            {
                var loyaltyPointsMapping = new Dictionary<Func<decimal, bool>, int>
        {
            { amount => amount < 10000000, 30 },
            { amount => amount >= 10000000 && amount < 40000000, 60 },
            { amount => amount >= 40000000 && amount < 2000000000, 120 },
            { amount => amount >= 2000000000, 1000 }
        };

                int loyaltyPoints = loyaltyPointsMapping.First(mapping => mapping.Key(orderSell.FinalAmount ?? 0)).Value;
                customer.LoyaltyPoints = (customer.LoyaltyPoints ?? 0) + loyaltyPoints;

                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();
            }

            // Update seller revenue
            var seller = await _accountRepository.GetByIdAsync(orderSell.SellerId);
            if (seller != null)
            {
                seller.Revenue = (seller.Revenue ?? 0) + orderSell.FinalAmount; // Update revenue
                _accountRepository.Update(seller);
                _accountRepository.SaveChanges();
            }

            var warrantyStartDate = DateTime.UtcNow;
            var warrantyEndDate = warrantyStartDate.AddYears(5); // cho 5 nam dai di 

            foreach (var orderSellDetail in orderSell.OrderSellDetails)
            {
                var warrantyTicket = new WarrantyTicket
                {
                    OrderSellDetailId = orderSellDetail.OrderSellDetailId,
                    ProductId = orderSellDetail.ProductId,
                    Status = "Active",
                    WarrantyStartDate = warrantyStartDate,
                    WarrantyEndDate = warrantyEndDate
                };

                await _warrantyTicketRepository.AddAsync(warrantyTicket);
            }
            _warrantyTicketRepository.SaveChanges();

            var orderSellResponse = _mapper.Map<OrderSellResponse>(orderSell);
            orderSellResponse.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            orderSellResponse.CustomerLoyaltyPoints = customer?.LoyaltyPoints;
            return orderSellResponse;
        }
        //Get Order By Customer
        public IEnumerable<OrderSellResponse> GetOrdersByCustomerId(string customerId)
        {
            var orders = _orderSellRepository.GetOrdersByCustomerId(customerId);
            var orderSellResponses = _mapper.Map<IEnumerable<OrderSellResponse>>(orders);

            foreach (var response in orderSellResponses)
            {
                var customer = _customerRepository.GetByIdAsync(response.CustomerId).Result;
                response.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            }

            return orderSellResponses;
        }


        public async Task<IEnumerable<OrderSellResponse>> GetOrderSells()
        {
            var orderSells = await _orderSellRepository.GetAllAsync();
            var orderSellResponses = _mapper.Map<IEnumerable<OrderSellResponse>>(orderSells);

            foreach (var response in orderSellResponses)
            {
                var customer = await _customerRepository.GetByIdAsync(response.CustomerId);
                response.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            }

            return orderSellResponses;
        }


        public async Task<IEnumerable<OrderSellResponse>> GetOrderSellDelivered()
        {
            var orderSells = await _orderSellRepository.GetAllOrderSellDeliveredAsync();
            var orderSellResponses = _mapper.Map<IEnumerable<OrderSellResponse>>(orderSells);

            foreach (var response in orderSellResponses)
            {
                var customer = await _customerRepository.GetByIdAsync(response.CustomerId);
                response.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            }

            return orderSellResponses;
        }


        public async Task<IEnumerable<OrderSellResponse>> GetOrderSellApproval()
        {
            var orderSells = await _orderSellRepository.GetAllOrderSellApprovalAsync();
            var orderSellResponses = _mapper.Map<IEnumerable<OrderSellResponse>>(orderSells);

            foreach (var response in orderSellResponses)
            {
                var customer = await _customerRepository.GetByIdAsync(response.CustomerId);
                response.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            }

            return orderSellResponses;
        }

        public async Task<IEnumerable<OrderSellResponse>> GetOrderSellApproved()
        {
            var orderSells = await _orderSellRepository.GetAllOrderSellApprovedAsync();
            var orderSellResponses = _mapper.Map<IEnumerable<OrderSellResponse>>(orderSells);

            foreach (var response in orderSellResponses)
            {
                var customer = await _customerRepository.GetByIdAsync(response.CustomerId);
                response.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            }

            return orderSellResponses;
        }

        public async Task<OrderSellResponse> GetOrderSellById(int orderSellId)
        {
            var orderSell = await _orderSellRepository.GetByIdAsync(orderSellId);
            var orderSellResponse = _mapper.Map<OrderSellResponse>(orderSell);
            var customer = await _customerRepository.GetByIdAsync(orderSell.CustomerId);
            orderSellResponse.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;

            return orderSellResponse;
        }


        public async Task<IEnumerable<OrderSellResponse>> GetOrderSellBySellerId(int sellerId)
        {
            var orderSells = await _orderSellRepository.GetAllOrderSellBySellerAsync(sellerId);
            var orderSellResponses = _mapper.Map<IEnumerable<OrderSellResponse>>(orderSells);

            foreach (var response in orderSellResponses)
            {
                var customer = await _customerRepository.GetByIdAsync(response.CustomerId);
                response.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            }

            return orderSellResponses;
        }

        public async Task<IEnumerable<OrderSellResponse>> GetOrderSellDeliveredBySellerId(int sellerId)
        {
            var orderSells = await _orderSellRepository.GetAllOrderSellDeliveredBySellerAsync(sellerId);
            var orderSellResponses = _mapper.Map<IEnumerable<OrderSellResponse>>(orderSells);

            foreach (var response in orderSellResponses)
            {
                var customer = await _customerRepository.GetByIdAsync(response.CustomerId);
                response.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            }

            return orderSellResponses;
        }

        public async Task<IEnumerable<OrderSellResponse>> GetOrderSellProcessingBySellerId(int sellerId)
        {
            var orderSells = await _orderSellRepository.GetAllOrderSellProcessingBySellerAsync(sellerId);
            var orderSellResponses = _mapper.Map<IEnumerable<OrderSellResponse>>(orderSells);

            foreach (var response in orderSellResponses)
            {
                var customer = await _customerRepository.GetByIdAsync(response.CustomerId);
                response.MemberShipDiscountPercent = customer?.Tier?.DiscountPercent;
            }

            return orderSellResponses;
        }
    }
}

