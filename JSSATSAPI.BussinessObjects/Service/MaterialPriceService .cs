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
        private readonly IMaterialTypeRepository _materialTypeRepository;
        private readonly IMaterialPriceRepository _materialPriceRepository;
        private readonly IMapper _mapper;

        public MaterialPriceService(IMaterialRepository materialRepository,IMaterialPriceRepository materialPriceRepository, IMapper mapper, IMaterialTypeRepository materialTypeRepository)
        {
            _materialRepository = materialRepository ?? throw new ArgumentNullException(nameof(materialRepository));
            _materialPriceRepository = materialPriceRepository ?? throw new ArgumentNullException(nameof(materialPriceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _materialTypeRepository = materialTypeRepository;
        }

        public async Task<List<MaterialPriceWithTypeResponse>> GetMaterialTypeWithDetailsAsync()
        {
            var materialTypes = await _materialTypeRepository.GetAllAsync();
            var materialTypeResponses = new List<MaterialPriceWithTypeResponse>();

            foreach (var materialType in materialTypes)
            {
                var materialTypeResponse = new MaterialPriceWithTypeResponse
                {
                    MaterialTypeId = materialType.MaterialTypeId,
                    MaterialTypeName = materialType.MaterialTypeName,
                    Materials = materialType.Materials.Select(m => new Material1Response
                    {
                        MaterialId = m.MaterialId,
                        MaterialName = m.MaterialName,
                        MaterialPrices = m.MaterialPrices.Select(mp => new MaterialPriceResponse
                        {
                            MaterialPriceId = mp.MaterialPriceId,
                            BuyPrice = mp.BuyPrice,
                            SellPrice = mp.SellPrice,
                            EffDate = mp.EffDate
                        }).ToList()
                    }).ToList()
                };

                materialTypeResponses.Add(materialTypeResponse);
            }

            return materialTypeResponses;
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
