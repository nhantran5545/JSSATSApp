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
    public class WarrantyTicketRepository : GenericRepository<WarrantyTicket>, IWarrantyTicketRepository
    {
        public WarrantyTicketRepository(JSS_DBContext context) : base(context)
        {
        }

        public async Task<List<WarrantyTicket>> GetActiveWarrantyTicketsAsync()
        {
            return await _context.WarrantyTickets
                .Where(t => t.Status == "Active")
                .ToListAsync();
        }

        public override async Task<IEnumerable<WarrantyTicket>> GetAllAsync()
        {
            return await _context.WarrantyTickets
                .Include(b => b.OrderSellDetail)
                .ThenInclude(b => b.Product)
                .ToListAsync();
        }
    }
}
