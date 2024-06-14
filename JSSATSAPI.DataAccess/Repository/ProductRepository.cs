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


        public override async Task<Product?> GetByIdAsync(object id)
        {
            return await _context.Products
                 .Include(b => b.Category)
                .Include(b => b.Counter)
                .Where(br => br.ProductId.Equals(id))
                .FirstOrDefaultAsync();
        }

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            return await _context.Products.Include(p => p.Category)
                                          .Include(p => p.Counter)
                                          .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task UpdateProductPrice(string productId, decimal productPrice, decimal buybackPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.ProductPrice = productPrice;
                product.BuyBackPrice = buybackPrice;
                await _context.SaveChangesAsync();
            }
        }
    }
}
