using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.DiamondRequest;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondPriceResponse;
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
            var customers = await _diamondRepository.GetAllDiamondsAvaiable();
            return _mapper.Map<IEnumerable<DiamondResponse>>(customers);
        }

        public async Task<DiamondWithPriceResponse> CreateDiamondWithPriceAsync(DiamondRequest request)
        {
            ValidateDiamondRequest(request);

            var diamond = _mapper.Map<Diamond>(request);
            diamond.DiamondCode = await _diamondRepository.GetNextDiamondCodeAsync();
            diamond.DiamondName = GenerateDiamondName(request);
            diamond.Status = "Active";

            var createdDiamond = await _diamondRepository.AddDiamondAsync(diamond);

            var existingDiamondPrice = await _diamondPriceRepository.GetDiamondPriceAsync(
                request.Origin,
                request.CaratWeightFrom,
                request.CaratWeightTo,
                request.Color,
                request.Clarity,
                request.Cut
            );

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


        public async Task<CheckPriceDiamond> CheckDiamondPriceAsync(CheckDiamondReq request)
        {
            ValidateDiamondRequest(request);

            var existingDiamondPrice = await _diamondPriceRepository.GetDiamondPriceAsync(
                request.Origin,
                request.CaratWeightFrom,
                request.CaratWeightTo,
                request.Color,
                request.Clarity,
                request.Cut
            );

            if (existingDiamondPrice != null)
            {
                return new CheckPriceDiamond
                {
                    Origin = existingDiamondPrice.Origin,
                    CaratWeightFrom = existingDiamondPrice.CaratWeightFrom,
                    CaratWeightTo = existingDiamondPrice.CaratWeightTo,
                    Color = existingDiamondPrice.Color,
                    Clarity = existingDiamondPrice.Clarity,
                    Cut = existingDiamondPrice.Cut,
                    SellPrice = existingDiamondPrice.SellPrice,
                    BuyPrice = existingDiamondPrice.BuyPrice,
                };
            }

            return new CheckPriceDiamond
            {
                Origin = request.Origin,
                CaratWeightFrom = request.CaratWeightFrom,
                CaratWeightTo = request.CaratWeightTo,
                Color = request.Color,
                Clarity = request.Clarity,
                Cut = request.Cut,
                SellPrice = 0,
                BuyPrice = 0,
            };
        }

        private void ValidateDiamondRequest(DiamondRequest request)
        {
            var validOrigins = new[] { "Nature", "Lab" };
            var validClarities = new[] { "IF", "VVS1", "VVS2", "VS1", "VS2" };
            var validCuts = new[] { "Good", "Excellent", "Very Excellent" };

            if (!Array.Exists(validOrigins, origin => origin.Equals(request.Origin, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Invalid Origin. Valid values are: Nature, Lab.");
            }

            if (!Array.Exists(validClarities, clarity => clarity.Equals(request.Clarity, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Invalid Clarity. Valid values are: IF, VVS1, VVS2, VS1, VS2.");
            }

            if (!Array.Exists(validCuts, cut => cut.Equals(request.Cut, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Invalid Cut. Valid values are: Good, Excellent, Very Excellent.");
            }
            if (request.CaratWeightFrom < 0 || request.CaratWeightTo < 0 || request.CaratWeightFrom >= request.CaratWeightTo)
            {
                throw new ArgumentException("Invalid carat weight range. 'CaratWeightFrom' must be less than 'CaratWeightTo' and both must be non-negative.");
            }
        }

        private void ValidateDiamondRequest(CheckDiamondReq request)
        {
            var validOrigins = new[] { "Nature", "Lab" };
            var validClarities = new[] { "IF", "VVS1", "VVS2", "VS1", "VS2" };
            var validCuts = new[] { "Good", "Excellent", "Very Excellent" };

            if (!Array.Exists(validOrigins, origin => origin.Equals(request.Origin, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Invalid Origin. Valid values are: Nature, Lab.");
            }

            if (!Array.Exists(validClarities, clarity => clarity.Equals(request.Clarity, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Invalid Clarity. Valid values are: IF, VVS1, VVS2, VS1, VS2.");
            }

            if (!Array.Exists(validCuts, cut => cut.Equals(request.Cut, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Invalid Cut. Valid values are: Good, Excellent, Very Excellent.");
            }

            if (request.CaratWeightFrom < 0 || request.CaratWeightTo < 0 || request.CaratWeightFrom >= request.CaratWeightTo)
            {
                throw new ArgumentException("Invalid carat weight range. 'CaratWeightFrom' must be less than 'CaratWeightTo' and both must be non-negative.");
            }
        }

        private string GenerateDiamondName(DiamondRequest request)
        {
            return $"Diamond {request.Origin} {request.CaratWeightFrom}CT-{request.CaratWeightTo}CT {request.Color} {request.Clarity} {request.Cut}";
        }
    }
}
