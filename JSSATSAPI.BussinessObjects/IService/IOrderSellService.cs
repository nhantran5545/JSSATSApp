using JSSATSAPI.BussinessObjects.RequestModels.OrderSellReq;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderSellResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IOrderSellService
    {
        Task<IEnumerable<OrderSellResponse>> GetOrderSells();
        Task<OrderSellResponse> GetOrderSellById(int orderSellId);
        OrderSellResponse CreateSellOrder(OrderSellRequest request);
        IEnumerable<OrderSellResponse> GetOrdersByCustomerId(string customerId);
        Task UpdateIndividualPromotionDiscountAsync(int orderSellId, decimal? newDiscount);
        Task<OrderSellResponse> PaidOrderSellAsync(CompletedOrderSellResponse completedOrderSellDto);
        Task<OrderSellResponse> CancelOrderSellAsync(int orderSellId);
        Task<OrderSellResponse> DeliveredOrderSellAsync(int orderSellId);
        Task<IEnumerable<OrderSellResponse>> GetOrderSellBySellerId(int sellerId);
        Task<IEnumerable<OrderSellResponse>> GetOrderSellDeliveredBySellerId(int sellerId);
        Task<IEnumerable<OrderSellResponse>> GetOrderSellProcessingBySellerId(int sellerId);
    }

}
