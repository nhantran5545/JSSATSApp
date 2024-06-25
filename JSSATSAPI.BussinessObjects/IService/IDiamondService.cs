using JSSATSAPI.BussinessObjects.RequestModels.DiamondRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IDiamondService
    {
        Task<DiamondWithPriceResponse> CreateDiamondWithPriceAsync(DiamondRequest request);
        Task<IEnumerable<DiamondResponse>> GetAllDiamond();
    }
}
