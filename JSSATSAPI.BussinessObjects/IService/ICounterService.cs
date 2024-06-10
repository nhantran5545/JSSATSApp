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
        Task<IEnumerable<CounterResponse>> GetAllCounters();
    }
}
