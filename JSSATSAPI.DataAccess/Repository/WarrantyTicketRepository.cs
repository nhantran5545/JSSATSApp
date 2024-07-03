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

        public override async Task<WarrantyTicket?> GetByIdAsync(object id)
        {
            return await _context.WarrantyTickets
                 .Include(b => b.Product)
                .Where(br => br.WarrantyId.Equals(id))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WarrantyTicket>> GetByPhoneNumberAsync(string phoneNumber)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == phoneNumber);
            if (customer == null) return new List<WarrantyTicket>();

            var orderSellDetails = await _context.OrderSellDetails
                .Where(osd => osd.OrderSell.CustomerId == customer.CustomerId)
                .ToListAsync();

            var warrantyTickets = new List<WarrantyTicket>();
            foreach (var detail in orderSellDetails)
            {
                var tickets = await _context.WarrantyTickets
                    .Where(wt => wt.OrderSellDetailId == detail.OrderSellDetailId)
                    .ToListAsync();

                warrantyTickets.AddRange(tickets);
            }

            return warrantyTickets;
        }
    }
}
