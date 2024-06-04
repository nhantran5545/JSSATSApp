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
    public class DiamondPriceRepository : GenericRepository<DiamondPrice>, IDiamondPriceRepository
    {
        public DiamondPriceRepository(JSS_DBContext context) : base(context)
        {
        }


        public async Task<DiamondPrice> GetLatestDiamondPriceAsync(string origin, decimal caratWeight, string color, string clarity, string cut)
        {
            return await _context.Set<DiamondPrice>()
                                 .Where(dp => dp.Origin == origin &&
                                              dp.CaratWeight == caratWeight &&
                                              dp.Color == color &&
                                              dp.Clarity == clarity &&
                                              dp.Cut == cut)
                                 .OrderByDescending(dp => dp.EffDate)
                                 .FirstOrDefaultAsync();
        }

    }
}
