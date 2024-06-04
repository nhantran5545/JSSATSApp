using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.Repository
{
    public class WarrantyTicketRepository : GenericRepository<WarrantyTicket>, IWarrantyTicketRepository
    {
        public WarrantyTicketRepository(JSS_DBContext context) : base(context)
        {
        }
    }
}
