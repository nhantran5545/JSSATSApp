using JSSATSAPI.BussinessObjects.RequestModels.AccountReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.AccountResponseModels;
using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.IService
{
    public interface IAccountService
    {
        Task<(string token, AccountResponse accountResponse, string errorMessage)> LoginAsync(AccountLoginRequest loginRequest);
        Task<AccountDetailResponse?> GetAccountByIdAsync(object accId);
        Task<bool> ChangStatusAccountById(int accId);
        int GetAccountIdFromToken();
        Task RegisterAccountAsync(AccountSignUpRequest accountSignUp);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<bool> UpdateAccount(int AccountId, AccountUpdateRequest accountUpdate);
        Task<bool> CheckUsernameExist(string username);
        Task<IEnumerable<AccountDetailResponse>> GetCashierAccountsAsync();
        Task<IEnumerable<AccountDetailResponse>> GetStaffAccountsAsync();
        bool IsManager(int accountId);
        bool IsSeller(int accountId);
        Task<ProfileResponse?> GetProfileAccountByIdAsync(int accId);
    }
}
