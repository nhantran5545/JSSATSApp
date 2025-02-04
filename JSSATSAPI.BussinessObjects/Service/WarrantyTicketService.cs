﻿using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.WarrantyTicketResponse;
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
    public class WarrantyTicketService : IWarrantyTicketService
    {
        private readonly IWarrantyTicketRepository _warrantyTicketRepository;
        private readonly IProductRepository  _productRepository;
        private readonly IMapper  _mapper;

        public WarrantyTicketService(IWarrantyTicketRepository warrantyTicketRepository, IMapper mapper , IProductRepository productRepository)
        {
            _warrantyTicketRepository = warrantyTicketRepository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<WarrantyTicketResponse>> GetAllWarrantyTicketsAsync()
        {
            var warrantyTickets = await _warrantyTicketRepository.GetAllAsync();
            var warrantyTicketResponses = new List<WarrantyTicketResponse>();
            foreach (var warrantyTicket in warrantyTickets)
            {
                var product = await _productRepository.GetByIdAsync(warrantyTicket.ProductId);
                var response = new WarrantyTicketResponse
                {
                    WarrantyId = warrantyTicket.WarrantyId,
                    OrderSellDetailId = warrantyTicket.OrderSellDetailId,
                    ProductId = warrantyTicket.ProductId,
                    ProductName = product?.ProductName ?? string.Empty, 
                    Status = warrantyTicket.Status,
                    WarrantyStartDate = warrantyTicket.WarrantyStartDate,
                    WarrantyEndDate = warrantyTicket.WarrantyEndDate
                };
                warrantyTicketResponses.Add(response);
            }
            return warrantyTicketResponses;
        }

        public async Task<WarrantyTicketResponse> GetWarrantyById(string warrantyId)
        {
            var warranty = await _warrantyTicketRepository.GetByIdAsync(warrantyId);
            return _mapper.Map<WarrantyTicketResponse>(warranty);
        }


        public async Task<IEnumerable<WarrantyTicketResponse>> GetWarrantyByPhoneNumberAsync(string phoneNumber)
        {
            var warrantyTickets = await _warrantyTicketRepository.GetByPhoneNumberAsync(phoneNumber);
            if(warrantyTickets == null)
            {
                throw new Exception("Phone Number Not Found");
            }
            var warrantyTicketResponses = new List<WarrantyTicketResponse>();
            foreach (var warrantyTicket in warrantyTickets)
            {
                var product = await _productRepository.GetByIdAsync(warrantyTicket.ProductId);
                var response = new WarrantyTicketResponse
                {
                    WarrantyId = warrantyTicket.WarrantyId,
                    OrderSellDetailId = warrantyTicket.OrderSellDetailId,
                    ProductId = warrantyTicket.ProductId,
                    ProductName = product?.ProductName ?? string.Empty,
                    Status = warrantyTicket.Status,
                    WarrantyStartDate = warrantyTicket.WarrantyStartDate,
                    WarrantyEndDate = warrantyTicket.WarrantyEndDate
                };
                warrantyTicketResponses.Add(response);
            }
            return warrantyTicketResponses;
        }
    }
}
