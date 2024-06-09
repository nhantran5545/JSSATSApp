using JSSATSAPI.BussinessObjects.RequestModels.ProductReq;
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
        Task<IEnumerable<ProductResponse>> GetAllProductMaterialsAndDiamondsAvaiableAsync();
        Task<IEnumerable<ProductResponse>> GetAllProductMaterialsAndDiamondsAsync();
        Task<ProductResponse> CreateProductAsync(AddProductRequest request);
        Task<ProductResponse> GetProductById(string productId);

        Task<decimal> CalculateBuyBackPriceForSingleProductAsync(string productId);
    }
}
