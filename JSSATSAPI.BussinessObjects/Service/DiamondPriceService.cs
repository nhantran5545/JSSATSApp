using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.Mapper;
using JSSATSAPI.BussinessObjects.RequestModels;
using JSSATSAPI.BussinessObjects.RequestModels.CustomerReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondPriceResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
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
    public class DiamondPriceService : IDiamondPriceService
    {

        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IMapper _mapper;

        public DiamondPriceService(IDiamondPriceRepository diamondPriceRepository, IMapper mapper)
        {
            _diamondPriceRepository = diamondPriceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DiamondPriceResponse>> GetAllDiamondPrsiceAsync()
        {
            var dp = await _diamondPriceRepository.GetAllAsync();
            return dp.Select(b => _mapper.Map<DiamondPriceResponse>(b)); 
        }

        public async Task<DiamondPriceResponse> CreateDiamondPriceAsync(DiamondPriceRequest request)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(request, null, null);
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                var errors = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Request is invalid: {errors}");
            }

            var newDiamondPrice = new DiamondPrice
            {
                Origin = request.Origin,
                CaratWeightFrom = request.CaratWeightFrom,
                CaratWeightTo = request.CaratWeightTo,
                Color = request.Color,
                Clarity = request.Clarity,
                Cut = request.Cut,
                SellPrice = request.SellPrice,
                BuyPrice = request.BuyPrice,
                EffDate = DateTime.Now
            };

            await _diamondPriceRepository.AddAsync(newDiamondPrice);
             _diamondPriceRepository.SaveChanges();

            var dpResponse = _mapper.Map<DiamondPriceResponse>(newDiamondPrice);
            return dpResponse;
        }

    }
}
