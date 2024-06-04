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
        Task<OrderSellResponse> GetOrderSellById(string orderSellId);
        OrderSellResponse CreateSellOrder(OrderSellRequest request);
        IEnumerable<OrderSellResponse> GetOrdersByCustomerId(string customerId);
        Task<OrderSellResponse> CompleteOrderSellAsync(CompletedOrderSellResponse completedOrderSellDto);
        Task<OrderSellResponse> CancelOrderSellAsync(int orderSellId);
    }

}
