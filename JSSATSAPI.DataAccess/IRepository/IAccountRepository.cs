using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.DataAccess.IRepository
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account> AuthenticateAsync(string username, string password);
        Task<IEnumerable<Account>> GetAccountsByRoleAsync(string role);
        Account GetAccountById(int accountId);
        Task<Account> GetByUsernameAsync(string username);
        Task<bool> SellerExistsAsync(int sellerId);
    }
}
