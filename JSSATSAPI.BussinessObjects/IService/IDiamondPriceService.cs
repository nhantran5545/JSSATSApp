using JSSATSAPI.BussinessObjects.RequestModels.DiamondRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondPriceResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IDiamondPriceService
    {
/*        Task<DiamondPriceResponse> CreateDiamondPriceAsync(DiamondPriceRequest request);*/
        Task<IEnumerable<DiamondPriceResponse>> GetAllDiamondPrsiceAsync();
        Task<DiamondPriceResponse> UpdateDiamondPriceAsync(int id, UpdateDiamondPriceRequest request);
    }
}
