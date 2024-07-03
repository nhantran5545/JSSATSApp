using JSSATSAPI.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IDiamondRepository : IGenericRepository<Diamond>
    {
        Task<Diamond> AddDiamondAsync(Diamond diamond);
        Task<IEnumerable<Diamond>> GetAllDiamondsAvaiable();
        Task<Diamond> GetDiamondByProductIdAsync(string productId);
        Task<string> GetNextDiamondCodeAsync();
    }
}
