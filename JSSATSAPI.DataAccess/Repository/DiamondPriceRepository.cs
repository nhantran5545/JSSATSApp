using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.Repository
{
    public class DiamondPriceRepository : GenericRepository<DiamondPrice>, IDiamondPriceRepository
    {
        public DiamondPriceRepository(JSS_DBContext context) : base(context)
        {
        }

        public async Task<DiamondPrice> GetDiamondPriceByIdAsync(int diamondPriceId)
        {
            return await _context.DiamondPrices.FindAsync(diamondPriceId);
        }
    }
}
