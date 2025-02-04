﻿using JSSATSAPI.BussinessObjects.RequestModels.MaterialReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialResponse;
using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IMaterialService
    {
        Task<MaterialWithPriceResponse> CreateMaterialWithPriceAsync(MaterialRequest request);
        Task DeleteMaterialAsync(int materialId);
        Task<IEnumerable<MaterialTypeResponse>> GetAllMaterialsWithTypesAsync();

    }
}
