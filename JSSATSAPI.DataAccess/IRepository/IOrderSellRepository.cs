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
        Task<IEnumerable<OrderSell>> GetAllOrderSellBySellerAsync(int sellerId);
        Task<IEnumerable<OrderSell>> GetAllOrderSellDeliveredBySellerAsync(int sellerId);
        Task<IEnumerable<OrderSell>> GetAllOrderSellProcessingBySellerAsync(int sellerId);
        Task<IEnumerable<OrderSell>> GetAllOrderSellDeliveredAsync();
        Task<IEnumerable<OrderSell>> GetAllOrderSellApprovalAsync();
        void SaveChanges();
        Task<IEnumerable<OrderSell>> GetAllOrderSellApprovedAsync();

        //Dashboard
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetRevenueByCategoryIdAsync(int categoryId);
        Task<List<OrderSell>> GetRecentOrdersAsync();
        Task<List<OrderSell>> GetOrdersByMonthAsync(int year, int month);
    }
}
