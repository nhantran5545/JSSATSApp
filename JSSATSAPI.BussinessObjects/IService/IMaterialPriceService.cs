﻿using JSSATSAPI.BussinessObjects.ResponseModels.DiamondResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialPriceResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.MaterialResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IMaterialPriceService
    {
        Task<IEnumerable<Material1Response>> GetAllMaterialsAsync();
    }
}
