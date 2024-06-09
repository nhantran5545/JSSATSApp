using JSSATSAPI.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<bool> CustomerExistsAsync(string customerId);
        Customer GetCustomerById(string customerId);
        
    }
}
