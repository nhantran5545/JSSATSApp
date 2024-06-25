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
    public class DiamondRepository : GenericRepository<Diamond>, IDiamondRepository
    {
        public DiamondRepository(JSS_DBContext context) : base(context)
        {
        }
        public async Task<Diamond> GetDiamondByProductIdAsync(string productId)
        {
            return await _context.Diamonds
                .FirstOrDefaultAsync(d => d.DiamondCode == productId);
        }

        public async Task<Diamond> AddDiamondAsync(Diamond diamond)
        {
            _context.Diamonds.Add(diamond);
            await _context.SaveChangesAsync();
            return diamond;
        }

        public async Task<string> GetNextDiamondCodeAsync()
        {
            var lastDiamond = await _context.Diamonds.OrderByDescending(d => d.DiamondCode).FirstOrDefaultAsync();
            if (lastDiamond == null)
            {
                return "DDDDW000001";
            }
            var lastCode = lastDiamond.DiamondCode;
            var numberPart = int.Parse(lastCode.Substring(5)) + 1;
            return $"DDDDW{numberPart:D6}";
        }
    }
}
