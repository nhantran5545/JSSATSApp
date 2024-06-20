using AutoMapper;
using JSSATSAPI.BussinessObjects.RequestModels.AccountReqModels;
using JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest;
using JSSATSAPI.BussinessObjects.RequestModels.OrderSellReq;
using JSSATSAPI.BussinessObjects.RequestModels.PaymentReq;
using JSSATSAPI.BussinessObjects.RequestModels.ProductReq;
using JSSATSAPI.BussinessObjects.ResponseModels;
using JSSATSAPI.BussinessObjects.ResponseModels.AccountResponseModels;
using JSSATSAPI.BussinessObjects.ResponseModels.CategoryResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.CounterResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondPriceResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialPriceResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackDetailRes;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
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
                        .ForMember(dest => dest.CounterName,
                                    opt => opt.MapFrom(src => src.Counter != null ? src.Counter.CounterName : string.Empty))
                        .ForMember(dest => dest.CategoryName,
                                    opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
                        .ForMember(dest => dest.DiscountRate,
                                    opt => opt.MapFrom(src => src.Category != null ? src.Category.DiscountRate : (decimal?)null))
                        .ReverseMap();
            CreateMap<OrderSell, OrderSellResponse>()
                        .ForMember(dest => dest.CustomerName,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
                        .ForMember(dest => dest.CustomerPhone,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Phone : string.Empty))
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
            //---------------------------------------------------
            // Mapping for OrderBuyBackOutOfStore
            CreateMap<OrderBuyBack, OrderBuyBackInStoreResponse>()
                         .ForMember(dest => dest.CustomerName,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
                         .ForMember(dest => dest.CustomerPhone,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Phone : string.Empty))
                         .ForMember(dest => dest.OrderBuyBackDetails,
                                    opt => opt.MapFrom(src => src.OrderBuyBackDetails))
                         .ReverseMap();

            // Mapping for OrderBuyBackDetailOutOfStore
            CreateMap<OrderBuyBackDetail, OrderBuyBackDetailInStoreResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
            //---------------------------------------------------
            // Mapping for OrderBuyBackInStore
            CreateMap<OrderBuyBack, OrderBuyBackResponse>()
                         .ForMember(dest => dest.CustomerName,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
                         .ForMember(dest => dest.CustomerPhone,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Phone : string.Empty))
                         .ForMember(dest => dest.OrderBuyBackDetails,
                                    opt => opt.MapFrom(src => src.OrderBuyBackDetails))
                         .ReverseMap();

            //---------------------------------------------------
            // Mapping for OrderBuyBackBoth
            CreateMap<OrderBuyBack, OrderBuyBackBothResponse>()
                         .ForMember(dest => dest.CustomerName,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
                         .ForMember(dest => dest.CustomerPhone,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Phone : string.Empty))
                         .ForMember(dest => dest.OrderBuyBackDetails,
                                    opt => opt.MapFrom(src => src.OrderBuyBackDetails))
                         .ReverseMap();

            // Mapping for OrderBuyBackBothDetail
            CreateMap<OrderBuyBackDetail, OrderBuyBackDetailBothResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));

            //---------------------------------------------------

            CreateMap<OrderSellDetail, OrderSellDetailResponse>()
                        .ForMember(dest => dest.ProductName,
                                    opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
                        .ForMember(dest => dest.ProductImage,
                                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Img : string.Empty))
                        .ReverseMap();
            CreateMap<Customer, CustomerResponse>()
                        .ForMember(dest => dest.TierName,
                                    opt => opt.MapFrom(src => src.Tier != null ? src.Tier.TierName : string.Empty))
                        .ForMember(dest => dest.DiscountPercent,
                                    opt => opt.MapFrom(src => src.Tier != null ? src.Tier.DiscountPercent : (int?)null))
                        .ReverseMap();
            CreateMap<Payment, PaymentResponse>()
                         .ForMember(dest => dest.PaymentTypeName,
                                    opt => opt.MapFrom(src => src.PaymentType != null ? src.PaymentType.PaymentTypeName : string.Empty))
                         .ReverseMap();
            CreateMap<PaymentType, PaymentTypeResponse>()
                         .ReverseMap();
            CreateMap<DiamondPrice, DiamondPriceResponse>()
                         .ReverseMap();


            CreateMap<OrderBuyBack, OrderBuyBackResponse>()
                         .ForMember(dest => dest.CustomerName,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
                         .ForMember(dest => dest.CustomerPhone,
                                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Phone : string.Empty))
                         .ForMember(dest => dest.OrderBuyBackDetails,
                                    opt => opt.MapFrom(src => src.OrderBuyBackDetails))
                          .ReverseMap();

            CreateMap<OrderBuyBackDetail, OrderBuyBackDetailResponse>()
                          .ReverseMap();
            CreateMap<MaterialType, MaterialTypeResponse>()
                          .ReverseMap();
            CreateMap<Material, MaterialResponse>()
                          .ReverseMap();
            CreateMap<Material, Material1Response>()
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MaterialId))
                .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.MaterialName))
                .ForMember(dest => dest.MaterialPrices, opt => opt.MapFrom(src => src.MaterialPrices))
                .ReverseMap();
            CreateMap<MaterialPrice, MaterialPriceResponse>()
                .ReverseMap();
            CreateMap<Counter, CounterResponse>()
                        .ForMember(dest => dest.FirstName,
                                    opt => opt.MapFrom(src => src.Account != null ? src.Account.FirstName : string.Empty))
                        .ForMember(dest => dest.LastName,
                                    opt => opt.MapFrom(src => src.Account != null ? src.Account.LastName : string.Empty))
                        .ReverseMap();
            CreateMap<Category, CategoryResponse>()
                .ReverseMap();

            CreateMap<CategoryType, CategoryTypeResponse>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories))
                .ReverseMap();
            #endregion
            #region Mapper_Request

            CreateMap<OrderSellRequest, OrderSell>()
                        .ForMember(dest => dest.OrderSellDetails, opt => opt.MapFrom(src => src.OrderSellDetails))
                        .ReverseMap();
            CreateMap<OrderSellDetailRequest, OrderSellDetail>()
                        .ReverseMap();
            CreateMap<Payment, PaymentRequest>()
                .ReverseMap();
            CreateMap<OrderBuyBackRequest, OrderBuyBack>()
                        .ForMember(dest => dest.OrderBuyBackDetails, opt => opt.MapFrom(src => src.OrderBuyBackDetails))
                        .ReverseMap();
            CreateMap<OrderBuyBackDetailRequest, OrderBuyBackDetailResponse>()
                .ReverseMap();

            CreateMap<OrderBuyBackDetailRequest, OrderBuyBackDetail>()
                .ForMember(dest => dest.BuyBackProductName, opt => opt.MapFrom(src => src.BuyBackProductName))
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MaterialId))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Origin))
                .ForMember(dest => dest.CaratWeight, opt => opt.MapFrom(src => src.CaratWeight))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.Clarity, opt => opt.MapFrom(src => src.Clarity))
                .ForMember(dest => dest.Cut, opt => opt.MapFrom(src => src.Cut))
                .ReverseMap();

            #endregion
        }
    }
}