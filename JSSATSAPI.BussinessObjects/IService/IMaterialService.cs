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
        Task<IEnumerable<MaterialTypeResponse>> GetAllMaterialsWithTypesAsync();

    }
}
