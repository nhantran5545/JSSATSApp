using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.OrderSellReq;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using JSSATSAPI.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
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
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public OrderSellService(IOrderSellRepository orderSellRepository, 
            IOrderSellDetailRepository orderSellDetailRepository, ICustomerRepository customerRepository, 
            IAccountRepository accountRepository, IMapper mapper, IProductRepository productRepository, IPaymentRepository paymentRepository)
        {
            _orderSellRepository = orderSellRepository;
            _orderSellDetailRepository = orderSellDetailRepository;
            _paymentRepository = paymentRepository;
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }
        //Create Sell Order
        public OrderSellResponse CreateSellOrder(OrderSellRequest request)
        {
            var orderSellDetails = _mapper.Map<List<OrderSellDetail>>(request.OrderSellDetails);
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

                product.Quantity -= orderSellDetail.Quantity;
                if (product.Quantity == 0)
                {
                    product.Status = "Chờ Thanh Toán";
                }
                _productRepository.Update(product);
            }

            if (orderSellDetails.Count == 0)
            {
                throw new Exception("No items in the order");
            }

            var finalAmount = totalAmount - (request.PromotionDiscount ?? 0) - (request.MemberShipDiscount ?? 0);

            var orderSell = new OrderSell
            {
                CustomerId = request.CustomerId,
                SellerId = request.SellerId,
                TotalAmount = totalAmount,
                PromotionDiscount = request.PromotionDiscount,
                MemberShipDiscount = request.MemberShipDiscount,
                FinalAmount = finalAmount,
                DiscountPercentForCustomer = request.DiscountPercentForCustomer,
                OrderDate = DateTime.Now,
                Status = "Processing",
                OrderSellDetails = orderSellDetails,
            };

            _orderSellRepository.AddOrderSell(orderSell);
            _orderSellRepository.SaveChanges();

            return _mapper.Map<OrderSellResponse>(orderSell);
        }


        public async Task<OrderSellResponse> CompleteOrderSellAsync(CompletedOrderSellResponse completedOrderSellDto)
        {
            var orderSell = await _orderSellRepository.GetByIdAsync(completedOrderSellDto.OrderSellId);
            if (orderSell == null)
            {
                return null;
            }

            foreach (var paymentDto in completedOrderSellDto.Payments)
            {
                var payment = _mapper.Map<Payment>(paymentDto);
                payment.OrderSellId = completedOrderSellDto.OrderSellId;
                payment.Amount = orderSell.FinalAmount;
                await _paymentRepository.AddPaymentAsync(payment);
            }

            orderSell.Status = "Completed";
             _orderSellRepository.Update(orderSell);
             _orderSellRepository.SaveChanges();

            var orderSellResponse = _mapper.Map<OrderSellResponse>(orderSell);
            return orderSellResponse;
        }

        public async Task<OrderSellResponse> CancelOrderSellAsync(int orderSellId)
        {
            var orderSell = await _orderSellRepository.GetByIdAsync(orderSellId);
            if (orderSell == null)
            {
                throw new Exception("Order not found");
            }

            foreach (var orderSellDetail in orderSell.OrderSellDetails)
            {
                var product = await _productRepository.GetByIdAsync(orderSellDetail.ProductId);
                if (product != null)
                {
                    product.Quantity += orderSellDetail.Quantity ?? 0;
                    _productRepository.Update(product);
                }
            }

            orderSell.Status = "Cancelled";
            _orderSellRepository.Update(orderSell);
            _orderSellRepository.SaveChanges();

            var orderSellResponse = _mapper.Map<OrderSellResponse>(orderSell);
            return orderSellResponse;
        }

        //Get Order By Customer
        public IEnumerable<OrderSellResponse> GetOrdersByCustomerId(string customerId)
        {
            var orders = _orderSellRepository.GetOrdersByCustomerId(customerId);
            return _mapper.Map<IEnumerable<OrderSellResponse>>(orders);
        }

        public async Task<IEnumerable<OrderSellResponse>> GetOrderSells()
        {
            var orderSell = await _orderSellRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderSellResponse>>(orderSell);
        }
        public async Task<OrderSellResponse> GetOrderSellById(string orderSellId)
        {
            var orderSell = await _orderSellRepository.GetByIdAsync(orderSellId);
            return _mapper.Map<OrderSellResponse>(orderSell);
        }

    }
}

