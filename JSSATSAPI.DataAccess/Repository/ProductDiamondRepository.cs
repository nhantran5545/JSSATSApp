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
    public class ProductDiamondRepository : GenericRepository<ProductDiamond>, IProductDiamondRepository
    {
        public ProductDiamondRepository(JSS_DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductDiamond>> GetAllProductDiamondsAsync()
        {
            return await _context.Set<ProductDiamond>()
                                 .Include(pd => pd.Product)
                                 .ThenInclude(p => p.Counter)
                                 .Include(pd => pd.Product)
                                 .ThenInclude(p => p.Category)
                                 .Include(pd => pd.DiamondCodeNavigation)
                                 .ToListAsync();
        }
        public async Task<IEnumerable<ProductDiamond>> GetAllProductDiamondsAvaiableAsync()
        {
            return await _context.Set<ProductDiamond>()
                                 .Include(pd => pd.Product)
                                 .ThenInclude(p => p.Counter)
                                 .Include(pd => pd.Product)
                                 .ThenInclude(p => p.Category)
                                 .Include(pd => pd.DiamondCodeNavigation)
                                  .Where(pd => pd.Product.Status == "Còn hàng")
                                 .ToListAsync();
        }
    }
}
