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
    public class CategoryTypeRepository : GenericRepository<CategoryType>, ICategoryTypeRepository
    {
        public CategoryTypeRepository(JSS_DBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<CategoryType>> GetAllAsync()
        {
            return await _context.CategoryTypes
                .Include(b => b.Categories)
                .ToListAsync();
        }
    }
}
