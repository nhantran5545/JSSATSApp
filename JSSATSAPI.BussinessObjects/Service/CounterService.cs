using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.CounterResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class CounterService : ICounterService
    {

        private readonly ICounterRepository _counterRepository;
        private readonly IMapper _mapper;

        public CounterService(ICounterRepository counterRepository, IMapper mapper)
        {
            _counterRepository = counterRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CounterResponse>> GetAllCounters()
        {
            var counter = await _counterRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CounterResponse>>(counter);
        }
    }
}
