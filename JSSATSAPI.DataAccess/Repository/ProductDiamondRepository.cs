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
    public class ProductDiamondRepository : GenericRepository<ProductDiamond>, IProductDiamondRepository
    {
        public ProductDiamondRepository(JSS_DBContext context) : base(context)
        {
        }
        public async Task AddProductDiamondAsync(ProductDiamond productDiamond)
        {
            // Construct the raw SQL query
            var sql = "INSERT INTO ProductDiamond (ProductId, DiamondCode) VALUES (@ProductId, @DiamondCode)";

            // Execute the raw SQL command
            await _context.Database.ExecuteSqlRawAsync(sql, new SqlParameter("@ProductId", productDiamond.ProductId), new SqlParameter("@DiamondCode", productDiamond.DiamondCode));
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
                                  .Where(pd => pd.Product.Status == "Còn hàng" && pd.Product.Quantity > 0)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ProductDiamond>> GetProductDiamondsByProductIdAsync(string productId)
        {
            return await _context.ProductDiamonds
                .Include(pd => pd.DiamondCodeNavigation)
                .Where(pd => pd.ProductId == productId)
                .ToListAsync();
        }
    }
}
