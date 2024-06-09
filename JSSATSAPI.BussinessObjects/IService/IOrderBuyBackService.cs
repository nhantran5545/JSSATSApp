using JSSATSAPI.BussinessObjects.RequestModels.OrderBuyBackRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.OrderBuyBackResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IOrderBuyBackService
    {
        Task<OrderBuyBackResponse> BuyBackProductOutOfStoreAsync(OrderBuyBackRequest request);
        Task<OrderBuyBackInStoreResponse> CreateOrderBuyBackInStoreAsync(OrderBuyBackInStoreRequest request);
    }
}
