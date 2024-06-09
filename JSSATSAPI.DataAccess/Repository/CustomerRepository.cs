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
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(JSS_DBContext context) : base(context)
        {
        }

        public async Task<bool> CustomerExistsAsync(string customerId)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
        }

        public override async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(b => b.Tier)
                .ToListAsync();
        }

        public override async Task<Customer?> GetByIdAsync(object id)
        {
            return await _context.Customers
                 .Include(b => b.Tier)
                .Where(br => br.CustomerId.Equals(id))
                .FirstOrDefaultAsync();
        }
        public Customer GetCustomerById(string customerId)
        {
            return _context.Customers.Include(c => c.Tier).FirstOrDefault(c => c.CustomerId == customerId);
        }

    }
}
