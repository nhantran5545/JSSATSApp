using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IOrderSellRepository : IGenericRepository<OrderSell>
    {
        void AddOrderSell(OrderSell orderSell);
        void AddOrderSellDetail(OrderSellDetail orderSellDetail);
        Product GetProductById(string productId);
        IEnumerable<OrderSell> GetOrdersByCustomerId(string customerId);
        Task<OrderSell> GetOrderSellWithDetailsAsync(int id);
        void SaveChanges();
    }
}
