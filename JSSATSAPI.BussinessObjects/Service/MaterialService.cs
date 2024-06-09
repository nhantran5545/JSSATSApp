using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
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
        private readonly IMaterialTypeRepository _materialTypeService;
        private readonly IMapper _mapper;

        public MaterialService(IMaterialTypeRepository materialTypeService, IMapper mapper)
        {
            _materialTypeService = materialTypeService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaterialTypeResponse>> GetAllMaterialsWithTypesAsync()
        {
            var materialTypes = await _materialTypeService.GetAllAsync();
            return _mapper.Map<IEnumerable<MaterialTypeResponse>>(materialTypes);
        }
    }
}
