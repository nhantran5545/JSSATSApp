using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.MaterialReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialResponse;
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
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialTypeRepository _materialType;
        private readonly IMaterialRepository _material;
        private readonly IMaterialPriceRepository _materialPrice;
        private readonly IMapper _mapper;

        public MaterialService(IMaterialTypeRepository materialType, IMapper mapper, IMaterialRepository material , IMaterialPriceRepository materialPrice)
        {
            _materialType = materialType;
            _material = material;
            _materialPrice = materialPrice;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaterialTypeResponse>> GetAllMaterialsWithTypesAsync()
        {
            var materialTypes = await _materialType.GetAllAsync();
            return _mapper.Map<IEnumerable<MaterialTypeResponse>>(materialTypes);
        }

        public async Task<MaterialWithPriceResponse> CreateMaterialWithPriceAsync(MaterialRequest request)
        {
            var material = new Material
            {
                MaterialName = request.MaterialName,
                MaterialTypeId = request.MaterialTypeId
            };

            var createdMaterial = await _material.AddMaterialAsync(material);

            var materialPrice = new MaterialPrice
            {
                MaterialId = createdMaterial.MaterialId,
                BuyPrice = request.BuyPrice,
                SellPrice = request.SellPrice,
                EffDate = DateTime.UtcNow
            };

            var createdMaterialPrice = await _materialPrice.AddMaterialPriceAsync(materialPrice);

            var response = new MaterialWithPriceResponse
            {
                MaterialId = createdMaterial.MaterialId,
                MaterialName = createdMaterial.MaterialName,
                MaterialTypeId = createdMaterial.MaterialTypeId,
                BuyPrice = createdMaterialPrice.BuyPrice,
                SellPrice = createdMaterialPrice.SellPrice,
                EffDate = createdMaterialPrice.EffDate
            };

            return response;
        }

    }
}
