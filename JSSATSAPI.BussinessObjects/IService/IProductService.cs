using JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllProductMaterialsAsync();
        Task<IEnumerable<ProductResponse>> GetAllProductDiamondsAsync();
        Task<IEnumerable<ProductResponse>> GetAllProductMaterialsAvaiableAsync();
        Task<IEnumerable<ProductResponse>> GetAllProductDiamondsAvaivaleAsync();

    }
}
