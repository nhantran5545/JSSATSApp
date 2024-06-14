using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.Mapper;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialPriceResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
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
    public class MaterialPriceService : IMaterialPriceService
    {
        private readonly IMaterialRepository _materialRepository;
        private readonly IMaterialPriceRepository _materialPriceRepository;
        private readonly IMapper _mapper;

        public MaterialPriceService(IMaterialRepository materialRepository,IMaterialPriceRepository materialPriceRepository, IMapper mapper)
        {
            _materialRepository = materialRepository ?? throw new ArgumentNullException(nameof(materialRepository));
            _materialPriceRepository = materialPriceRepository ?? throw new ArgumentNullException(nameof(materialPriceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Material1Response>> GetAllMaterialsAsync()
        {
            var materials = await _materialRepository.GetAllMaterialsAsync();
            if (materials == null)
            {
                throw new Exception("No materials found");
            }
            return _mapper.Map<IEnumerable<Material1Response>>(materials);
        }

        public async Task UpdateMaterialPriceAsync(int materialPriceId, decimal buyPrice, decimal sellPrice, DateTime effDate)
        {
            if (effDate < DateTime.Today)
            {
                throw new ArgumentException("Effective date must be greater than or equal to the current date.");
            }

            var materialPrice = await _materialPriceRepository.GetByIdAsync(materialPriceId);
            if (materialPrice == null)
            {
                throw new Exception($"MaterialPrice with ID {materialPriceId} not found.");
            }

            materialPrice.BuyPrice = buyPrice;
            materialPrice.SellPrice = sellPrice;
            materialPrice.EffDate = effDate;

             _materialPriceRepository.Update(materialPrice);
            _materialPriceRepository.SaveChanges();
        }
    }
}
