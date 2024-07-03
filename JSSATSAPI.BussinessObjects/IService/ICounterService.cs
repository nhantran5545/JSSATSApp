using JSSATSAPI.BussinessObjects.RequestModels.CounterRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.CounterResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface ICounterService
    {
        Task<CounterResponse> CreateCounterAsync(CreateCounteRequest request);
        Task<IEnumerable<CounterResponse>> GetAllCounters();
        Task<CounterResponse> GetCounterById(int counter);
        Task<bool> UpdateCounterAsync(int counterId, CreateCounteRequest request);
    }
}
