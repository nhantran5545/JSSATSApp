using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.Mapper;
using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialPriceResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialResponse;
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

    }
}
