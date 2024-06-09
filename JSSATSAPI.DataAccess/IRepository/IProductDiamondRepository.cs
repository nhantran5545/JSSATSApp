using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IProductDiamondRepository : IGenericRepository<ProductDiamond>
    {
        Task AddProductDiamondAsync(ProductDiamond productDiamond);
        Task<IEnumerable<ProductDiamond>> GetAllProductDiamondsAsync();
        Task<IEnumerable<ProductDiamond>> GetAllProductDiamondsAvaiableAsync();
        Task<IEnumerable<ProductDiamond>> GetProductDiamondsByProductIdAsync(string productId);
    }
}
