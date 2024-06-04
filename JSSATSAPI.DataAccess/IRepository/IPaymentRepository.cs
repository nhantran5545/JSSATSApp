using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task AddPaymentAsync(Payment payment);
    }
}
