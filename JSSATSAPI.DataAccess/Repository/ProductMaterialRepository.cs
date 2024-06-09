using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using Microsoft.Data.SqlClient;
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
        public async Task AddProductMaterialAsync(ProductMaterial productMaterial)
        {
            // Construct the raw SQL query
            var sql = "INSERT INTO ProductMaterial (ProductId, MaterialId, Weight) VALUES (@ProductId, @MaterialId, @Weight)";

            // Create parameters ensuring that null values are handled appropriately
            var productIdParam = new SqlParameter("@ProductId", productMaterial.ProductId);
            var materialIdParam = productMaterial.MaterialId.HasValue
                ? new SqlParameter("@MaterialId", productMaterial.MaterialId.Value)
                : new SqlParameter("@MaterialId", DBNull.Value);
            var weightParam = new SqlParameter("@Weight", productMaterial.Weight);

            // Execute the raw SQL command
            await _context.Database.ExecuteSqlRawAsync(sql, productIdParam, materialIdParam, weightParam);
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
                                 .Include(pm => pm.Material)
                                 .Where(pm => pm.Product.Status == "Còn hàng" && pm.Product.Quantity > 0)
                                 .ToListAsync();
        }


        public MaterialPrice GetMaterialPriceById(int materialId)
        {
            return _context.MaterialPrices.FirstOrDefault(mp => mp.MaterialId == materialId);
        }

        public IEnumerable<ProductMaterial> GetProductMaterialsByProductId(string productId)
        {
            return _context.ProductMaterials
                                 .Include(pm => pm.Product)
                                 .ThenInclude(p => p.Counter)
                                 .Include(pm => pm.Product)
                                 .ThenInclude(p => p.Category)
                                 .Include(pm => pm.Material)
                .Where(pm => pm.ProductId == productId).ToList();
        }

        public async Task<MaterialPrice> GetLatestMaterialPriceAsync(int materialId)
        {
            return await _context.Set<MaterialPrice>()
                .Include(p => p.Material)
                                 .Where(mp => mp.MaterialId == materialId)
                                 .OrderByDescending(mp => mp.EffDate)
                                 .FirstOrDefaultAsync();
        }
    }
}
