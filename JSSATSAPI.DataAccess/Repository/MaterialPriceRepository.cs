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
    public class MaterialPriceRepository : GenericRepository<MaterialPrice>, IMaterialPriceRepository
    {
        public MaterialPriceRepository(JSS_DBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<MaterialPrice>> GetAllAsync()
        {
            return await _context.MaterialPrices
                .Include(b => b.Material)
                .ToListAsync();
        }

    }
}
