using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IMaterialRepository : IGenericRepository<Material>
    {
        Task<Material> AddMaterialAsync(Material material);
        Task DeleteAsync(Material material);
        Task<IEnumerable<Material>> GetAllMaterialsAsync();
    }
}
