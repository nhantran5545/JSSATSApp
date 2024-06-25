using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.CounterRequest;
using JSSATSAPI.BussinessObjects.RequestModels.CustomerReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.CounterResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using JSSATSAPI.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class CounterService : ICounterService
    {

        private readonly ICounterRepository _counterRepository;
        private readonly IAccountRepository  _accountRepository;
        private readonly IMapper _mapper;

        public CounterService(ICounterRepository counterRepository, IMapper mapper , IAccountRepository accountRepository)
        {
            _counterRepository = counterRepository;
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<CounterResponse>> GetAllCounters()
        {
            var counter = await _counterRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CounterResponse>>(counter);
        }

        public async Task<CounterResponse> CreateCounterAsync(CreateCounteRequest request)
        {
            if (!Validator.TryValidateObject(request, new ValidationContext(request), null, true))
            {
                throw new ValidationException("Request is invalid");
            }
            var accountId = await _accountRepository.GetByIdAsync(request.AccountId);
            if (accountId == null)
            {
                throw new Exception($"Category with ID {request.AccountId} not found");
            }
            var newCustomer = new Counter
            {
                CounterName = request.CounterName,
                AccountId = request.AccountId,
            };
            await _counterRepository.AddAsync(newCustomer);
            _counterRepository.SaveChanges();


            var customerResponse = _mapper.Map<CounterResponse>(newCustomer);

            return customerResponse;
        }

        public async Task UpdateCounterAsync(int counterId, CreateCounteRequest updateCounterDto)
        {
            var counter = await _counterRepository.GetByIdAsync(counterId);
            if (counter == null)
            {
                throw new Exception("Counter not found");
            }

            _mapper.Map(updateCounterDto, counter);

            _counterRepository.Update(counter);
            _counterRepository.SaveChanges();
        }
    }
}
