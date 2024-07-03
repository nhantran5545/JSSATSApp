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
    public class MaterialPriceRepository : GenericRepository<MaterialPrice>, IMaterialPriceRepository
    {
        public MaterialPriceRepository(JSS_DBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<MaterialPrice>> GetAllAsync()
        {
            return await _context.MaterialPrices
                .Include(b => b.Material)
                .ThenInclude(b => b.MaterialType)
                .ToListAsync();
        }
        public async Task<IEnumerable<MaterialPrice>> GetByMaterialIdAsync(int materialId)
        {
            return await _context.MaterialPrices.Where(mp => mp.MaterialId == materialId).ToListAsync();
        }
        public async Task<MaterialPrice> AddMaterialPriceAsync(MaterialPrice materialPrice)
        {
            var result = await _context.MaterialPrices.AddAsync(materialPrice);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
        public async Task DeleteAsync(MaterialPrice materialPrice)
        {
            _context.MaterialPrices.Remove(materialPrice);
            await _context.SaveChangesAsync();
        }
    }
}
