using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IWarrantyTicketRepository : IGenericRepository<WarrantyTicket>
    {
        Task<List<WarrantyTicket>> GetActiveWarrantyTicketsAsync();
    }
}
