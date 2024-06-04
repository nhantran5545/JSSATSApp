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
    public class DiamondRepository : GenericRepository<Diamond>, IDiamondRepository
    {
        public DiamondRepository(JSS_DBContext context) : base(context)
        {
        }
        public async Task<Diamond> GetDiamondByProductIdAsync(string productId)
        {
            return await _context.Diamonds
                .FirstOrDefaultAsync(d => d.DiamondCode == productId);
        }
    }
}
