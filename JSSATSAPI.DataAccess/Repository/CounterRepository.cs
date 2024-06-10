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
    public class CounterRepository : GenericRepository<Counter>, ICounterRepository
    {
        public CounterRepository(JSS_DBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Counter>> GetAllAsync()
        {
            return await _context.Counters
                .Include(b => b.Account)
                .ToListAsync();
        }
    }
}
