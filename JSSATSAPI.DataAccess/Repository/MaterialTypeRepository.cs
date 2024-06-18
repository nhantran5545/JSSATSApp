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
    public class MaterialTypeRepository : GenericRepository<MaterialType>, IMaterialTypeRepository
    {
        public MaterialTypeRepository(JSS_DBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<MaterialType>> GetAllAsync()
        {
            return await _context.MaterialTypes
                .Include(b => b.Materials)
                .ThenInclude(m => m.MaterialPrices)
                .ToListAsync();
        }
    }
}
