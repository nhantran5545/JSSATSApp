using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        IEnumerable<Product> GetProductsByStatus();
        MaterialPrice GetMaterialPriceById(int materialId);
        IEnumerable<ProductMaterial> GetProductMaterialsByProductId(string productId);
        Task UpdateProductPrice(string productId, decimal productPrice);

        //Diamond


    }
}
