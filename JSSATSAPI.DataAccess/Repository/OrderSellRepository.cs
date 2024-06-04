using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.Repository
{
    public class OrderSellRepository : GenericRepository<OrderSell>, IOrderSellRepository
    {
        public OrderSellRepository(JSS_DBContext context) : base(context)
        {

        }

        public override async Task<IEnumerable<OrderSell>> GetAllAsync()
        {
            return await _context.OrderSells
                .Include(b => b.Customer)
                .Include(b => b.Seller)
                .Include(b => b.OrderSellDetails)
                .ToListAsync();
        }


        public override async Task<OrderSell?> GetByIdAsync(object id)
        {
            return await _context.OrderSells
                .Include(b => b.Customer)
                .Include(b => b.Seller)
                .Include(b => b.OrderSellDetails)
                .Where(br => br.OrderSellId.Equals(id))
                .FirstOrDefaultAsync();
        }

        public async Task<OrderSell> GetOrderSellWithDetailsAsync(int id)
        {
            return await _context.OrderSells
                .Include(b => b.Customer)
                .Include(b => b.Seller)
                .Include(b => b.Payments)
                .ThenInclude(b => b.PaymentType)
                .Include(os => os.OrderSellDetails)
                .ThenInclude(b => b.Product)
                .Include(os => os.Payments)
                .FirstOrDefaultAsync(os => os.OrderSellId == id);
        }
        public void AddOrderSell(OrderSell orderSell)
        {
            _context.OrderSells.Add(orderSell);
        }

        public void AddOrderSellDetail(OrderSellDetail orderSellDetail)
        {
            _context.OrderSellDetails.Add(orderSellDetail);
        }

        public Product GetProductById(string productId)
        {
            return _context.Products.SingleOrDefault(p => p.ProductId == productId);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public IEnumerable<OrderSell> GetOrdersByCustomerId(string customerId)
        {
            return _context.OrderSells
                .Include(o => o.OrderSellDetails)
                .Where(o => o.CustomerId == customerId)
                .ToList();
        }
    }
}
