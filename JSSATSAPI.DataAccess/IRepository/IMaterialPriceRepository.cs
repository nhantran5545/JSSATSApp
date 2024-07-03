using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IMaterialPriceRepository : IGenericRepository<MaterialPrice>
    {
        Task<MaterialPrice> AddMaterialPriceAsync(MaterialPrice materialPrice);
        Task DeleteAsync(MaterialPrice materialPrice);
        Task<IEnumerable<MaterialPrice>> GetByMaterialIdAsync(int materialId);
    }
}
