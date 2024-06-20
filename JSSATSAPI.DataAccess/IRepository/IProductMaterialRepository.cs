using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IProductMaterialRepository : IGenericRepository<ProductMaterial>
    {
        Task AddProductMaterialAsync(ProductMaterial productMaterial);
        Task<IEnumerable<ProductMaterial>> GetAllProductMaterials();
        Task<IEnumerable<ProductMaterial>> GetAllProductMaterialsAvaiable();

        MaterialPrice GetMaterialPriceById(int materialId);

        IEnumerable<ProductMaterial> GetProductMaterialsByProductId(string productId);
        Task<MaterialPrice> GetLatestMaterialPriceAsync(int materialId);
        Task DeleteProductMaterialsByProductIdAsync(string productId);
    }
}
