using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IPaymentTypeService
    {
        Task<IEnumerable<PaymentTypeResponse>> GetAllPaymentTypeAsync();
    }
}
