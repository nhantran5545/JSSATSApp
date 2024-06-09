using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.Mapper;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondPriceResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class DiamondPriceService : IDiamondPriceService
    {

        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IMapper _mapper;

        public DiamondPriceService(IDiamondPriceRepository diamondPriceRepository, IMapper mapper)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DiamondPriceResponse>> GetAllDiamondPriceAsync()
        {
            var dp = await _diamondPriceRepository.GetAllAsync();
            return dp.Select(b => _mapper.Map<DiamondPriceResponse>(b)); 
        }
    }
}
