using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponse>> GetAllCustomers();
        Task<CustomerResponse> GetCustomerById(string customerId);
    }
}
