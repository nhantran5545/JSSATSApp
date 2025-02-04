﻿using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.AccountReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.AccountResponseModels;
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
            try
            {
                var (token, account, errorMessage) = await _accountService.LoginAsync(loginRequest);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return Unauthorized(errorMessage);
                }

                return Ok(new { token, account });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
                if ( accountSignUp.Role != "Seller" && accountSignUp.Role != "Cashier")
                {
                    BadRequest("Invalid role. Only 'Seller' or 'Cashier' roles are allowed.");
                }
                await _accountService.RegisterAccountAsync(accountSignUp);
                return Ok("Account registered successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{accountId}")]
        [Authorize]
        public async Task<IActionResult> UpdateAccount(int accountId, [FromBody] AccountUpdateRequest accountUpdate)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return NotFound("Account not found");
            }

            if (await _accountService.UpdateAccount(accountId, accountUpdate))
            {
                return Ok(accountUpdate);
            }
            return BadRequest("Something wrong with the server Please try again");
        }

        [HttpGet("sellers")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetSellerAccounts()
        {
            try
            {
                var sellers = await _accountService.GetStaffAccountsAsync();
                return Ok(sellers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("cashier")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetCashierAccounts()
        {
            try
            {
                var sellers = await _accountService.GetCashierAccountsAsync();
                return Ok(sellers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("allAccount")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllAccount()
        {
            try
            {
                var acc = await _accountService.GetAllAccountsAsync();
                return Ok(acc);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ProfileResponse>> GetProfileAccountById()
        {
            var accId = _accountService.GetAccountIdFromToken();
            var profileDetail = await _accountService.GetProfileAccountByIdAsync(accId);
            return Ok(profileDetail);
        }

        [HttpPut("status/{accountId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ChangStatusAccount(int accountId)
        {
            try
            {
                var account = await _accountService.ChangStatusAccountById(accountId);

                if (account == null)
                {
                    return NotFound("Account not found.");
                }

                if (!User.IsInRole("Manager"))
                {
                    return Forbid(); // Returns 403 Forbidden status code
                }

                if (account)
                {
                    return Ok("Change status successfully.");
                }
                return NotFound($"Account with ID {accountId} not found.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
