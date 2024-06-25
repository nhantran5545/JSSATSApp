using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.DiamondRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using JSSATSAPI.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class DiamondService : IDiamondService
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IMapper _mapper;

        public DiamondService(IDiamondRepository diamondRepository , IMapper mapper , IDiamondPriceRepository diamondPriceRepository)
        {
            _diamondRepository = diamondRepository;
            _mapper = mapper;
            _diamondPriceRepository = diamondPriceRepository;
        }

        public async Task<IEnumerable<DiamondResponse>> GetAllDiamond()
        {
            var customers = await _diamondRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DiamondResponse>>(customers);
        }

        public async Task<DiamondWithPriceResponse> CreateDiamondWithPriceAsync(DiamondRequest request)
        {
            var diamond = _mapper.Map<Diamond>(request);
            diamond.DiamondCode = await _diamondRepository.GetNextDiamondCodeAsync();
            diamond.DiamondName = GenerateDiamondName(request);
            diamond.Status = "Active";

            var createdDiamond = await _diamondRepository.AddDiamondAsync(diamond);

            var existingDiamondPrice = await _diamondPriceRepository.GetDiamondPriceAsync(request.Origin, request.CaratWeightFrom, request.CaratWeightTo, request.Color, request.Clarity, request.Cut);

            DiamondPrice createdDiamondPrice;

            if (existingDiamondPrice == null)
            {
                var diamondPrice = _mapper.Map<DiamondPrice>(request);
                diamondPrice.EffDate = DateTime.UtcNow;

                createdDiamondPrice = await _diamondPriceRepository.AddDiamondPriceAsync(diamondPrice);
            }
            else
            {
                createdDiamondPrice = existingDiamondPrice;
            }

            var response = _mapper.Map<DiamondWithPriceResponse>(createdDiamond);
            response.SellPrice = createdDiamondPrice.SellPrice;
            response.BuyPrice = createdDiamondPrice.BuyPrice;
            response.EffDate = createdDiamondPrice.EffDate;

            return response;
        }



        private string GenerateDiamondName(DiamondRequest request)
        {
            return $"Diamond {request.Origin} {request.CaratWeightFrom}CT-{request.CaratWeightTo}CT {request.Color} {request.Clarity} {request.Cut}";
        }
    }
}
