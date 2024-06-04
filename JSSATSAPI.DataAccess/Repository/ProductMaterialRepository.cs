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
    public class ProductMaterialRepository : GenericRepository<ProductMaterial>, IProductMaterialRepository
    {
        public ProductMaterialRepository(JSS_DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductMaterial>> GetAllProductMaterials()
        {
            return _context.ProductMaterials
                           .Include(pd => pd.Product)
                           .ThenInclude(pd => pd.Counter)
                           .Include(pd => pd.Product)
                           .ThenInclude(pd => pd.Category)
                           .ToList();
        }

        public async Task<IEnumerable<ProductMaterial>> GetAllProductMaterialsAvaiable()
        {
            return await _context.ProductMaterials
                                 .Include(pm => pm.Product)
                                 .ThenInclude(p => p.Counter)
                                 .Include(pm => pm.Product)
                                 .ThenInclude(p => p.Category)
                                 .Where(pm => pm.Product.Status == "Còn hàng")
                                 .ToListAsync();
        }


        public MaterialPrice GetMaterialPriceById(int materialId)
        {
            return _context.MaterialPrices.FirstOrDefault(mp => mp.MaterialId == materialId);
        }

        public IEnumerable<ProductMaterial> GetProductMaterialsByProductId(string productId)
        {
            return _context.ProductMaterials.Where(pm => pm.ProductId == productId).ToList();
        }
    }
}
