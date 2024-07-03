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
    public class OrderBuyBackRepository : GenericRepository<OrderBuyBack>, IOrderBuyBackRepository
    {
        public OrderBuyBackRepository(JSS_DBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<OrderBuyBack>> GetAllAsync()
        {
            return await _context.OrderBuyBacks
                .Include(b => b.Customer)
                .Include(b => b.Payments)
                .ThenInclude(b => b.PaymentType)
                .Include(b => b.OrderBuyBackDetails)
                .ThenInclude(b => b.Product)
                .ToListAsync();
        }

        public override async Task<OrderBuyBack?> GetByIdAsync(object id)
        {
            return await _context.OrderBuyBacks
                .Include(b => b.Customer)
                .Include(b => b.Payments)
                .ThenInclude(b => b.PaymentType)
                .Include(b => b.OrderBuyBackDetails)
                .ThenInclude(b => b.Product)
                .Where(br => br.OrderBuyBackId.Equals(id))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderBuyBack>> GetAllOrderBuyBacPaidAsync()
        {
            return await _context.OrderBuyBacks
                .Where(br => br.Status == "Paid")
                .ToListAsync();
        }
    }
}
