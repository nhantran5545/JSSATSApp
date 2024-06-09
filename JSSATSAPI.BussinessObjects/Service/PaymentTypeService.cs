using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
using JSSATSAPI.DataAccess.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class PaymentTypeService : IPaymentTypeService
    {
        private readonly IPaymentTypeRepository _paymentTypeRepository;
        private readonly IMapper _mapper;

        public PaymentTypeService(IPaymentTypeRepository paymentTypeRepository, IMapper mapper)
        {
            _paymentTypeRepository = paymentTypeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentTypeResponse>> GetAllPaymentTypeAsync()
        {
            var paymentTypes = await _paymentTypeRepository.GetAllAsync();
            return paymentTypes.Select(b => _mapper.Map<PaymentTypeResponse>(b));
        }
    }
}
