using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.AccountReqModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JSSATS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AccountLoginRequest loginRequest)
        {
            var (token, account, errorMessage) = await _accountService.LoginAsync(loginRequest);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Unauthorized(errorMessage);
            }

            return Ok(new { token, account });
        }

        [HttpPost("signup")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> SignUp(AccountSignUpRequest accountSignUp)
        {
            try
            {

                if (await _accountService.CheckUsernameExist(accountSignUp.Username))
                {
                    BadRequest("Username already exists");
                }
                if (accountSignUp.Role != "Manager" && accountSignUp.Role != "Staff" && accountSignUp.Role != "Cashier")
                {
                    BadRequest("Invalid role. Only 'Manager' , 'Staff' or 'Cashier' roles are allowed.");
                }
                await _accountService.RegisterAccountAsync(accountSignUp);
                return Ok("Account registered successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
