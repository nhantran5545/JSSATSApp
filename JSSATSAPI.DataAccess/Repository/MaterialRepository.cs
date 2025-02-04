﻿using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.Repository
{
    public class MaterialRepository : GenericRepository<Material>, IMaterialRepository
    {
        public MaterialRepository(JSS_DBContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Material>> GetAllMaterialsAsync()
        {
            return await _context.Materials
                .Include(m => m.MaterialPrices)
                .ToListAsync();
        }
        public async Task<Material> AddMaterialAsync(Material material)
        {
            var result = await _context.Materials.AddAsync(material);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
        public async Task DeleteAsync(Material material)
        {
            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();
        }
    }
}
