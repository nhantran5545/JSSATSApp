using AutoMapper;
using JSSATSAPI.BussinessObjects.RequestModels.AccountReqModels;
using JSSATSAPI.BussinessObjects.RequestModels.OrderSellReq;
using JSSATSAPI.BussinessObjects.RequestModels.PaymentReq;
using JSSATSAPI.BussinessObjects.ResponseModels;
using JSSATSAPI.BussinessObjects.ResponseModels.AccountResponseModels;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels;
using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Mapper
{
    public class Mapper : Profile
    {
        public Mapper() 
        {
            #region Mapper_Response
            CreateMap<Account, AccountDetailResponse>()
                      .ReverseMap();
            CreateMap<Account, AccountResponse>()
                       .ReverseMap();

            CreateMap<Product, ProductResponse>()
                        .ForMember(dest => dest.ProductPrice, opt => opt.Ignore())
                        .ForMember(dest => dest.CounterName,
                                    opt => opt.MapFrom(src => src.Counter != null ? src.Counter.CounterName : string.Empty))
                        .ForMember(dest => dest.CategoryName,
                                    opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
                        .ReverseMap();
            CreateMap<OrderSell, OrderSellResponse>()
                        .ForMember(dest => dest.CustomerName,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
                        .ForMember(dest => dest.SellerFirstName,
                                    opt => opt.MapFrom(src => src.Seller != null ? src.Seller.FirstName : string.Empty))
                        .ForMember(dest => dest.SellerLastName,
                                    opt => opt.MapFrom(src => src.Seller != null ? src.Seller.LastName : string.Empty))
                        .ForMember(dest => dest.OrderSellDetails, 
                                    opt => opt.MapFrom(src => src.OrderSellDetails))
                        .ReverseMap();
            CreateMap<OrderSell, SellInvoiceDTO>()
                        .ForMember(dest => dest.CustomerName,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
                        .ForMember(dest => dest.SellerFirstName,
                                    opt => opt.MapFrom(src => src.Seller != null ? src.Seller.FirstName : string.Empty))
                        .ForMember(dest => dest.SellerLastName,
                                    opt => opt.MapFrom(src => src.Seller != null ? src.Seller.LastName : string.Empty))
                        .ForMember(dest => dest.OrderSellDetails,
                                    opt => opt.MapFrom(src => src.OrderSellDetails))
                        .ReverseMap();
            CreateMap<OrderSellDetail, OrderSellDetailResponse>()
                        .ForMember(dest => dest.ProductName,
                                    opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
                        .ReverseMap();
            CreateMap<Customer, CustomerResponse>()
                        .ForMember(dest => dest.TierName,
                                    opt => opt.MapFrom(src => src.Tier != null ? src.Tier.TierName : string.Empty))
                         .ReverseMap();
            CreateMap<Payment, PaymentResponse>()
                         .ForMember(dest => dest.PaymentTypeName,
                                    opt => opt.MapFrom(src => src.PaymentType != null ? src.PaymentType.PaymentTypeName : string.Empty))
                         .ReverseMap();
            #endregion
            #region Mapper_Request
            CreateMap<OrderSellRequest, OrderSell>()
                        .ForMember(dest => dest.OrderSellDetails, opt => opt.MapFrom(src => src.OrderSellDetails))
                        .ReverseMap();
            CreateMap<OrderSellDetailRequest, OrderSellDetail>()
                        .ReverseMap();
            CreateMap<Payment, PaymentReq>()
                .ReverseMap();
            #endregion

        }
    }
}