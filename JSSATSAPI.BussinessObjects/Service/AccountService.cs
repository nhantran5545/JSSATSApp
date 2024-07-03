using AutoMapper;
using JSSATSAPI.BussinessObjects.InheritanceClass;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.AccountReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.AccountResponseModels;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountService(IAccountRepository accountRepository, IConfiguration configuration, IMemoryCache memoryCache, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            ProvideToken.Initialize(_configuration, _memoryCache);
        }
        //login service
        public async Task<(string token, AccountResponse accountResponse, string errorMessage)> LoginAsync(AccountLoginRequest loginRequest)
        {
            var account = await _accountRepository.AuthenticateAsync(loginRequest.Username, loginRequest.Password);

            if (account != null)
            {
                if (account.Status != "InActive")
                {
                    var token = ProvideToken.Instance.GenerateToken(account.AccountId, account.Role);

                    var accountResponse = _mapper.Map<AccountResponse>(account);

                    return (token.token, accountResponse, null);
                }
                else
                {
                    return (null, null, "Your account is not active, please contact Admin");
                }
            }
            else
            {
                return (null, null, "Username or password are not correct");
            }
        }
        //check username
        public async Task<bool> CheckUsernameExist(string username)
        {
            var existingAccount = await _accountRepository.GetByUsernameAsync(username);
            if (existingAccount != null)
            {
                throw new ArgumentException("Username already exists");
            }
            return false;
        }
        //getbyid
        public async Task<AccountDetailResponse?> GetAccountByIdAsync(object accId)
        {
            var acc = await _accountRepository.GetByIdAsync(accId);
            return _mapper.Map<AccountDetailResponse>(acc);
        }
        //getAccIdFromToken method
        public int GetAccountIdFromToken()
        {
            int result = 0;
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.User.Identity.IsAuthenticated)
            {
                var accountIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "AccountId");
                if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return result = accountId;
                }
            }
            return result;
        }
        // get All account service
        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        public async Task<ProfileResponse?> GetProfileAccountByIdAsync(int accId)
        {
            var acc = await _accountRepository.GetByIdAsync(accId);
            return _mapper.Map<ProfileResponse>(acc);
        }

        public async Task RegisterAccountAsync(AccountSignUpRequest accountSignUp)
        {
            var validationContext = new ValidationContext(accountSignUp, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(accountSignUp, validationContext, validationResults, validateAllProperties: true))
            {
                var validationErrors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
                throw new ArgumentException($"Validation failed: {validationErrors}");
            }

            var account = _mapper.Map<Account>(accountSignUp);
            account.Status = "Active";

            await _accountRepository.AddAsync(account);
            _accountRepository.SaveChanges();
        }


        public async Task<bool> UpdateAccount(int AccountId, AccountUpdateRequest accountUpdate)
        {
            var account = await _accountRepository.GetByIdAsync(AccountId);
            if (account == null)
            {
                return false;
            }

            // Update only if the new values are not null or empty
            if (!string.IsNullOrEmpty(accountUpdate.FirstName))
            {
                account.FirstName = accountUpdate.FirstName;
            }
            if (!string.IsNullOrEmpty(accountUpdate.LastName))
            {
                account.LastName = accountUpdate.LastName;
            }
            if (!string.IsNullOrEmpty(accountUpdate.Phone))
            {
                account.Phone = accountUpdate.Phone;
            }
            if (!string.IsNullOrEmpty(accountUpdate.Email))
            {
                account.Email = accountUpdate.Email;
            }
            if (!string.IsNullOrEmpty(accountUpdate.ImageUrl))
            {
                account.ImageUrl = accountUpdate.ImageUrl;
            }
            if (!string.IsNullOrEmpty(accountUpdate.Address))
            {
                account.Address = accountUpdate.Address;
            }

            _accountRepository.Update(account);
            var result = _accountRepository.SaveChanges();
            if (result < 1)
            {
                return false;
            }
            return true;
        }



        public async Task<IEnumerable<AccountDetailResponse>> GetCashierAccountsAsync()
        {
            var managerAccounts = await _accountRepository.GetAccountsByRoleAsync("Cashier");
            return _mapper.Map<IEnumerable<AccountDetailResponse>>(managerAccounts);
        }

        public async Task<IEnumerable<AccountDetailResponse>> GetStaffAccountsAsync()
        {
            var managerAccounts = await _accountRepository.GetAccountsByRoleAsync("Seller");
            return _mapper.Map<IEnumerable<AccountDetailResponse>>(managerAccounts);
        }

        public bool IsManager(int accountId)
        {
            var account = _accountRepository.GetAccountById(accountId);
            return account != null && account.Role == "Manager";
        }
        public bool IsSeller(int accountId)
        {
            var account = _accountRepository.GetAccountById(accountId);
            return account != null && account.Role == "Staff";
        }

        public async Task<bool> ChangStatusAccountById(int accId)
        {

            var account = await _accountRepository.GetByIdAsync(accId);
            if (account == null)
            {
                return false;
            }

            account.Status = account.Status == "Active" ? "InActive" : "Active";

            var result = _accountRepository.SaveChanges();
            if (result < 1)
            {
                return false;
            }
            return true;
        }
    }
}
