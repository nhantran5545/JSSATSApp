using JSSATSAPI.BussinessObjects.ResponseModels.PaymentTypeResponse;
using JSSATSAPI.BussinessObjects.ResponseModels.WarrantyTicketResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IWarrantyTicketService
    {
        Task<IEnumerable<WarrantyTicketResponse>> GetAllWarrantyTicketsAsync();
        Task<WarrantyTicketResponse> GetWarrantyById(string warrantyId);
        Task<IEnumerable<WarrantyTicketResponse>> GetWarrantyByPhoneNumberAsync(string phoneNumber);
    }
}
