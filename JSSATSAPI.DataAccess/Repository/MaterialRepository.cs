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
    public class MaterialRepository : GenericRepository<Material>, IMaterialRepository
    {
        public MaterialRepository(JSS_DBContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Material>> GetAllMaterialsAsync()
        {
            return await _context.Materials
                .Include(m => m.MaterialPrices)
                .ToListAsync();
        }
    }
}
