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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(JSS_DBContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(b => b.Category)
                .Include(b => b.Counter)
                .ToListAsync();
        }

        public IEnumerable<Product> GetProductsByStatus()
        {
            return _context.Products
                .Include(b => b.Category)
                .Include(b => b.Counter)
                .Where(p => p.Status == "Còn hàng").ToList(); 
        }

        public MaterialPrice GetMaterialPriceById(int materialId)
        {
            return _context.MaterialPrices.FirstOrDefault(mp => mp.MaterialId == materialId);
        }

        public IEnumerable<ProductMaterial> GetProductMaterialsByProductId(string productId)
        {
            return _context.ProductMaterials.Where(pm => pm.ProductId == productId).ToList();


        }
        public async Task UpdateProductPrice(string productId, decimal productPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.ProductPrice = productPrice;
                await _context.SaveChangesAsync();
            }
        }



    }
}
