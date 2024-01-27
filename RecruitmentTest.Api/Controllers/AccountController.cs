using Microsoft.AspNetCore.Mvc;
using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain.Dtos.Account;
using RecruitmentTest.Domain.Helpers;
using RecruitmentTest.Domain.Models;
using RecruitmentTest.Services.IServices;
using System.Data;
using System.Net;

namespace RecruitmentTest.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            if (!ModelState.IsValid)
            {
                response.ErrorMessages.AddRange(GetErrorMessagesHelper.GetErrorMessages(ModelState));
                return BadRequest(response);
            }
            else
            {
                response = await accountService.Login(model);
                if (response.IsSuccess) return Ok(response);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            if (!ModelState.IsValid)
            {
                response.ErrorMessages.AddRange(GetErrorMessagesHelper.GetErrorMessages(ModelState));
                return BadRequest(response);
            }
            else
            {
                response = await accountService.Register(model);
                if (response.IsSuccess) return Ok(response);
                return BadRequest(response);
            }
        }

    }
}